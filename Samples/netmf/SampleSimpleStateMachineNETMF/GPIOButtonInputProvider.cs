using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Input;

namespace SampleSimpleStateMachineNETMF
{
    internal class GPIOButtonInputProvider
    {
        public delegate void GPIOButtonInputHandler(InputReportArgs arg);

        private readonly Dispatcher m_dispatcher;
        private ButtonPad[] m_buttons;
        private GPIOButtonInputHandler m_buttonInputHandler;

        public GPIOButtonInputProvider(GPIOButtonInputHandler buttonInputHandler)
        {
            if (buttonInputHandler == null)
            {
                throw new ArgumentNullException("buttonInputHandler");
            }
            m_buttonInputHandler = buttonInputHandler;
            m_dispatcher = Dispatcher.CurrentDispatcher;

            // Create a hardware provider.
            HardwareProvider hwProvider = new HardwareProvider();

            // Create the pins that are needed for the buttons.
            // Default their values for the emulator.
            Cpu.Pin pinLeft = Cpu.Pin.GPIO_Pin0;
            Cpu.Pin pinRight = Cpu.Pin.GPIO_Pin1;
            Cpu.Pin pinUp = Cpu.Pin.GPIO_Pin2;
            Cpu.Pin pinSelect = Cpu.Pin.GPIO_Pin3;
            Cpu.Pin pinDown = Cpu.Pin.GPIO_Pin4;

            // Use the hardware provider to get the pins.  If the left pin is  
            // not set, assume none of the pins are set, and set the left pin  
            // back to the default emulator value. 
            if ((pinLeft = hwProvider.GetButtonPins(Button.VK_LEFT)) == Cpu.Pin.GPIO_NONE)
            {
                pinLeft = Cpu.Pin.GPIO_Pin0;
            }
            else
            {
                pinRight = hwProvider.GetButtonPins(Button.VK_RIGHT);
                pinUp = hwProvider.GetButtonPins(Button.VK_UP);
                pinSelect = hwProvider.GetButtonPins(Button.VK_SELECT);
                pinDown = hwProvider.GetButtonPins(Button.VK_DOWN);
            }

            // Allocate button pads and assign the (emulated) hardware pins as input from specific buttons.
            m_buttons = new ButtonPad[]
                        {
                            // Associate the buttons to the pins as discovered or set above.
                            new ButtonPad(this, Button.VK_LEFT  , pinLeft),
                            new ButtonPad(this, Button.VK_RIGHT , pinRight),
                            new ButtonPad(this, Button.VK_UP    , pinUp),
                            new ButtonPad(this, Button.VK_SELECT, pinSelect),
                            new ButtonPad(this, Button.VK_DOWN  , pinDown),
                        };
        }


        private class ButtonPad : IDisposable
        {
            private Button m_button;
            private InterruptPort m_port;
            private GPIOButtonInputProvider m_sink;
            private ButtonDevice m_buttonDevice;

            public ButtonPad(GPIOButtonInputProvider sink, Button button, Cpu.Pin pin)
            {
                m_sink = sink;
                m_button = button;
                m_buttonDevice = InputManager.CurrentInputManager.ButtonDevice;

                // Do not set an InterruptPort with GPIO_NONE. 
                if (pin != Cpu.Pin.GPIO_NONE)
                {
                    // When this GPIO pin is true, call the Interrupt method.
                    m_port = new InterruptPort(pin,
                                                true,
                                                Port.ResistorMode.PullUp,
                                                Port.InterruptMode.InterruptEdgeBoth);
                    m_port.OnInterrupt += new NativeEventHandler(this.Interrupt);
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    if (m_port != null)
                    {
                        m_port.Dispose();
                        m_port = null;
                    }
                }
                // Free native resources.
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            void Interrupt(uint data1, uint data2, DateTime time)
            {
                RawButtonActions action = (data2 != 0) ? RawButtonActions.ButtonUp : RawButtonActions.ButtonDown;
                RawButtonInputReport report = new RawButtonInputReport(null, time, m_button, action);

                // Queue the button press to the handler.
                m_sink.m_dispatcher.BeginInvoke(delegate(object arg)
                                              {
                                                  m_sink.m_buttonInputHandler((InputReportArgs)arg);
                                                  return null;
                                              },
                                              new InputReportArgs(m_buttonDevice, report));
            }
        }
    }
}
