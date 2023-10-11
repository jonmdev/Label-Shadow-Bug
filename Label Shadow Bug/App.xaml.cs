using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;
using System.Diagnostics;

namespace Label_Shadow_Bug {
    public partial class App : Application {

        public event Action screenSizeChanged = null;
        public double screenWidth = 0;
        public double screenHeight = 0;
        ContentPage mainPage;
        public App() {

            InitializeComponent();

            //=========
            //LAYOUT
            //=========
            mainPage = new();
            mainPage.Background = Colors.SaddleBrown;
            MainPage = mainPage;
            mainPage.SizeChanged += delegate {
                invokeScreenSizeChangeEvent();
            };

            AbsoluteLayout abs = new();

            VerticalStackLayout vert = new();
            mainPage.Content = abs;

            abs.Add(vert);

            NestedBorder nestedBorder = new();
            nestedBorder = new();
            nestedBorder.HorizontalOptions = LayoutOptions.Center;
            nestedBorder.innerBorder.Padding = new Thickness(4, 0);
            nestedBorder.Margin = new Thickness(0, 4);
            nestedBorder.BackgroundColor = Colors.DarkGoldenrod;
            nestedBorder.innerBorder.BackgroundColor = Colors.Coral;
            nestedBorder.setOuterCornerRadius(12);
            nestedBorder.setOuterPadding(5);
            vert.Add(nestedBorder);

            Label label = new();
            label.Text = "HELLO WORLD";
            label.TextColor = Colors.White;
            label.FontSize = 23; //changing font size can fix bug
            label.FontFamily = "MontserratBold";
            label.Shadow = new() { Offset = new Point(0, 1), Radius = 1, Brush = Colors.Aqua }; //disabling shadow can fix bug
            label.MaxLines = 1;
            label.LineBreakMode = LineBreakMode.NoWrap;
            label.HorizontalOptions = LayoutOptions.Center;
            nestedBorder.innerBorder.Content = label;
            //vert.Children.Add(label); //moving the label to the vert also fixes the bug

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

                label2.Shadow = new() { Offset = new Point(0, androidView.Context.ToPixels(5)), Radius = androidView.Context.ToPixels(7) };
#endif
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
    public class NestedBorder : Border {
        public Border innerBorder;
        Thickness outerPadding;
        CornerRadius outerCornerRadius;
        
        public NestedBorder() {
            innerBorder = new();
            this.Content = innerBorder;
        }
        public void setOuterPadding(Thickness outerPadding) {
            this.outerPadding = outerPadding;
            updatePaddingAndRadiusUniform();
        }
        public void setOuterCornerRadius(CornerRadius outerCornerRadius) {
            this.outerCornerRadius = outerCornerRadius;
            updatePaddingAndRadiusUniform();
        }
        private void updatePaddingAndRadiusUniform() {
            this.Padding = outerPadding;
            this.StrokeShape = new RoundRectangle() { CornerRadius = outerCornerRadius };
            //this.StrokeThickness = 0; //enabling this line fixes bug also
            innerBorder.StrokeShape = new RoundRectangle() { CornerRadius = (outerCornerRadius.TopLeft - outerPadding.Left) };
            //innerBorder.StrokeThickness = 0; //enabling this line fixes bug also
        }

    }
}