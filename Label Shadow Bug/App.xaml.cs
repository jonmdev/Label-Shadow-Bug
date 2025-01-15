#if ANDROID
using Android.Content;
#endif
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System.Diagnostics;

namespace Label_Shadow_Bug {
#if ANDROID
    class CMauiTextView: MauiTextView {
        
        bool applyFix = false; //TO APPLY TEMP FIX FOR TEXT CLIPPING IN ANDROID, VISIBLE ON PIXEL 5 EMULATOR

        public CMauiTextView(Context context) : base(context) {
            System.Diagnostics.Debug.WriteLine("CREATE CMAUITEXTVIEW");
        }
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {

            //https://stackoverflow.com/questions/14493732/what-are-widthmeasurespec-and-heightmeasurespec-in-android-custom-views

            var measured = Paint.MeasureText(Text);

            int widthSize = widthMeasureSpec.GetSize();
            Android.Views.MeasureSpecMode widthMode = widthMeasureSpec.GetMode();

            if (applyFix) {
                widthSize += 1;
            }

            int newWidthSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(widthSize, widthMode);

            var scaledDensity = DeviceDisplay.Current.MainDisplayInfo.Density; //= 2.75 on Pixel 5 emulator
            System.Diagnostics.Debug.WriteLine("LABEL: " + Text + " ANDROID WIDTH RAW " + + widthSize + " SCALED " + widthSize / scaledDensity);

            base.OnMeasure(newWidthSpec, heightMeasureSpec); 
        }
    }
#endif
    public partial class App : Application {

        public event Action screenSizeChanged = null;
        public double screenWidth = 0;
        public double screenHeight = 0;
        ContentPage mainPage;

        
        public App() {

#if ANDROID
            LabelHandler.PlatformViewFactory = (handler) => {
                return new CMauiTextView(handler.Context); //Create custom MauiTextView for CLabelHandler
            };
#endif 

            //=========
            //LAYOUT
            //=========
            mainPage = new();
            mainPage.Background = Colors.MediumAquamarine;
            MainPage = mainPage;
            mainPage.SizeChanged += delegate {
                invokeScreenSizeChangeEvent();
            };

            AbsoluteLayout abs = new();
            mainPage.Content = abs;

            VerticalStackLayout vert = new();
            abs.Add(vert);

            Label label = new();
            label.Text = "CHAT MISSING_WORD";
            label.TextColor = Colors.White;
            label.FontSize = 23; //changing font size can fix bug
            label.FontFamily = "MontserratBold";
            label.Shadow = new() { Offset = new Point(0, 1), Radius = 1, Brush = Colors.Aqua }; 
            label.MaxLines = 1;
            label.LineBreakMode = LineBreakMode.NoWrap;
            label.HorizontalOptions = LayoutOptions.Center;

            Border newBorder = new();
            newBorder.HorizontalOptions = LayoutOptions.Center;
            newBorder.Content = label;
            newBorder.BackgroundColor = Colors.DarkCyan;
            newBorder.Padding = new Thickness(6, 2);
            newBorder.Margin = new Thickness(0, 4);
            newBorder.StrokeShape = new RoundRectangle() { CornerRadius = 12 };
            newBorder.StrokeThickness = 0.5;
            newBorder.HandlerChanged += delegate {
#if ANDROID
                Android.Views.View androidView = ElementExtensions.ToPlatform(newBorder, newBorder.Handler.MauiContext);
                newBorder.Shadow = new() { Offset = new Point(0, androidView.Context.ToPixels(5)), Radius = androidView.Context.ToPixels(4) }; //android needs re-scaling of shadows due to other reported bug elsewhere
#else
                newBorder.Shadow = new() { Offset = new Point(0, 5), Radius = 4 };
#endif
            };
            
            newBorder.Content = label;
            vert.Add(newBorder);
            
            Label label2 = new();
            label2.Text = "ALEXANDRA";
            label2.FontFamily = "NotoSansBold";
            label2.FontSize = 42;
            label2.HorizontalOptions = LayoutOptions.Center;
            label2.TextColor = Colors.White;
            label2.Margin = new Thickness(0, -5, 0, 10);
            label2.HandlerChanged += delegate {
#if ANDROID
                Android.Views.View androidView = ElementExtensions.ToPlatform(label2, label2.Handler.MauiContext);
                label2.Shadow = new() { Offset = new Point(0, androidView.Context.ToPixels(5)), Radius = androidView.Context.ToPixels(7) }; //android needs re-scaling of shadows due to other reported bug elsewhere
#else
                label2.Shadow = new() { Offset = new Point(0, 5), Radius = 7 };
#endif
            };
            label2.SizeChanged += delegate {
                Debug.WriteLine("LABEL: " + label2.Text + " SIZE W " + label2.Width + " H " + label2.Height);
            };
            vert.Add(label2);

            //==================
            //RESIZE FUNCTION
            //==================
            screenSizeChanged += delegate {
                vert.HeightRequest = screenHeight;
                vert.WidthRequest = screenWidth;
            };
            Debug.WriteLine("FINISHED BUILD OKAY");

        }
        private void invokeScreenSizeChangeEvent() {
            if (mainPage.Width > 0 && mainPage.Height > 0) {
                screenWidth = mainPage.Width;
                screenHeight = mainPage.Height;
                Debug.WriteLine("main page size changed | width: " + screenWidth + " height: " + screenHeight);
                screenSizeChanged?.Invoke();

                
            }
        }
    }
    
}