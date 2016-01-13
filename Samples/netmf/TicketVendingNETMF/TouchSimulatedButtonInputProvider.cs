using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Touch;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Hardware;

namespace TicketVendingNETMF
{
    public class TouchSimulatedButtonInputProvider
    {
        private InputProviderSite site;
        private Dispatcher dispatcher;
        private PresentationSource presentationSource;
        private ButtonDevice buttonDevice;

        public TouchSimulatedButtonInputProvider(PresentationSource presentationSource)
        {
            this.presentationSource = presentationSource;
            site = InputManager.CurrentInputManager.RegisterInputProvider(this);

            this.dispatcher = Dispatcher.CurrentDispatcher;
            this.buttonDevice = InputManager.CurrentInputManager.ButtonDevice;

            TouchScreen touchScreen = new TouchScreen(new TouchScreen.ActiveRectangle[] {
                new TouchScreen.ActiveRectangle(0, SystemMetrics.ScreenHeight / 3, (2 * SystemMetrics.ScreenWidth) / 3, SystemMetrics.ScreenHeight / 3, Button.VK_SELECT),
                new TouchScreen.ActiveRectangle((2 * SystemMetrics.ScreenWidth) / 3, SystemMetrics.ScreenHeight / 3, SystemMetrics.ScreenWidth / 3, SystemMetrics.ScreenHeight / 3, Button.VK_RIGHT),
                new TouchScreen.ActiveRectangle(0, 0, SystemMetrics.ScreenWidth, SystemMetrics.ScreenHeight / 3, Button.VK_UP),
                new TouchScreen.ActiveRectangle(0, (2 * SystemMetrics.ScreenHeight) / 3, SystemMetrics.ScreenWidth, SystemMetrics.ScreenHeight / 3, Button.VK_DOWN),
            });
            Touch.Initialize(touchScreen);
            TouchCollectorConfiguration.CollectionMethod = CollectionMethod.Native;
            TouchCollectorConfiguration.CollectionMode = CollectionMode.InkAndGesture;
            //TouchCollectorConfiguration.SamplingFrequency = 20000;
            //TouchCollectorConfiguration.TouchMoveFrequency = 5;
            touchScreen.OnTouchUp += new TouchScreenEventHandler(this.TouchScreen_OnTouchUp);
            touchScreen.OnTouchDown += new TouchScreenEventHandler(this.TouchScreen_OnTouchDown);
        }

        private void TouchScreen_OnTouchUp(object sender, TouchScreenEventArgs eventArgs)
        {
            this.HandleTouch(RawButtonActions.ButtonUp, eventArgs);
        }

        private void TouchScreen_OnTouchDown(object sender, TouchScreenEventArgs eventArgs)
        {
            this.HandleTouch(RawButtonActions.ButtonDown, eventArgs);
        }

        private void HandleTouch(RawButtonActions action, TouchScreenEventArgs eventArgs)
        {
            dispatcher.Invoke(
                        new TimeSpan(0, 0, 3),
                        delegate(object arg)
                        {
                            Button button = (Button)eventArgs.Target;
                            RawButtonInputReport report = new RawButtonInputReport(this.presentationSource, eventArgs.TimeStamp, button, action);
                            bool handled = site.ReportInput(this.buttonDevice, report);
                            if (!handled)
                            {
                                ButtonEventArgs buttonEventArgs = new ButtonEventArgs(this.buttonDevice, this.presentationSource, eventArgs.TimeStamp, button);
                                this.OnButtonUp(buttonEventArgs);
                            }
                            return null;
                        },
                        null);

        }

        public event RoutedEventHandler ButtonUp;

        protected virtual void OnButtonUp(RoutedEventArgs eventArgs)
        {
            RoutedEventHandler buttonUp = this.ButtonUp;
            if (buttonUp != null)
            {
                buttonUp(this, eventArgs);
            }
        }
    }
}
