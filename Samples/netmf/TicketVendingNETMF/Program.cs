using System;
using System.Collections;

using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Touch;
using Microsoft.SPOT.Hardware;

using StaMa;


namespace TicketVendingNETMF
{
    public class Program : Microsoft.SPOT.Application
    {
        private Window mainWindow;
        private Text infoTextbox;
        private Text remainingAmountTextbox;
        private Text outputSlot;
        private ListBox productItemsListBox;
        private ListBox coinSlotsListBox;
        private StateMachine stateMachine;
        private DispatcherTimer introTimeout;
        private DispatcherTimer processTimeout;
        private DispatcherTimer deliverTimeout;
        private int depositBalance;
        private ProductItem acquiredProduct;
        private UIElement mainPage;
        private UIElement introPage;

        //## Begin StateNames
        // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "TicketVendingNETMF"
        // at 07-22-2015 22:02:46 using StaMaShapes Version 2300
        private static readonly string Intro = "Intro";
        private static readonly string Idle = "Idle";
        private static readonly string ItemSelected = "ItemSelected";
        private static readonly string Empty = "Empty";
        private static readonly string EmitRequired = "EmitRequired";
        private static readonly string CoinInserted = "CoinInserted";
        private static readonly string CloseBargain = "CloseBargain";
        private static readonly string Delivering = "Delivering";
        //## End StateNames

        private static readonly string DeliverTimeout = "DeliverTimeout";
        private static readonly string ButtonSelect = "ButtonSelect";
        private static readonly string ButtonCancel = "ButtonCancel";
        private static readonly string IntroTimeout = "IntroTimeout";
        private static readonly string ProcessTimeout = "ProcessTimeout";

        private static readonly string nothingSelectedText = "----";


        public static void Main()
        {
            Program myApplication = new Program();

            myApplication.CreateUI();

            myApplication.InitStateMachine();

            try
            {
                GPIOButtonInputProvider inputProvider = new GPIOButtonInputProvider(null);
            }
            catch (Exception ex)
            {
                Debug.Print(String.Concat("Buttons not available.", ex.Message));
            }

            try
            {
                TouchSimulatedButtonInputProvider touchInputProvider = new TouchSimulatedButtonInputProvider(null);
                touchInputProvider.ButtonUp += myApplication.MainWindow_ButtonUp;
            }
            catch (Exception ex)
            {
                Debug.Print(String.Concat("Touch not available.", ex.Message));
            }

            myApplication.Run(myApplication.mainWindow);
        }


        public void CreateUI()
        {
            mainWindow = new Window();
            mainWindow.Width = SystemMetrics.ScreenWidth;
            mainWindow.Height = SystemMetrics.ScreenHeight;

            Font smallFont = Resources.GetFont(Resources.FontResources.small);

            this.infoTextbox = new Text();
            this.infoTextbox.TextWrap = true;
            this.infoTextbox.TextAlignment = Microsoft.SPOT.Presentation.Media.TextAlignment.Center;
            this.infoTextbox.Font = smallFont;
            this.infoTextbox.TextContent = String.Empty;
            this.infoTextbox.HorizontalAlignment = Microsoft.SPOT.Presentation.HorizontalAlignment.Center;
            this.infoTextbox.VerticalAlignment = Microsoft.SPOT.Presentation.VerticalAlignment.Center;

            this.productItemsListBox = new ListBox();
            AddProductItem(productItemsListBox, "Town Center", 80);
            AddProductItem(productItemsListBox, "Airport", 150);
            AddProductItem(productItemsListBox, "Leisure Park", 30);

            this.coinSlotsListBox = new ListBox();
            AddCoinSlot(coinSlotsListBox, 10);
            AddCoinSlot(coinSlotsListBox, 20);
            AddCoinSlot(coinSlotsListBox, 50);
            AddCoinSlot(coinSlotsListBox, 100);

            Text destinationLabel = new Text(smallFont, "Destination:");
            destinationLabel.HorizontalAlignment = Microsoft.SPOT.Presentation.HorizontalAlignment.Left;
            destinationLabel.VerticalAlignment = Microsoft.SPOT.Presentation.VerticalAlignment.Center;
            destinationLabel.SetMargin(5, 5, 5, 5);

            Text priceLabel = new Text(smallFont, "Missing Amount:");
            priceLabel.HorizontalAlignment = Microsoft.SPOT.Presentation.HorizontalAlignment.Left;
            priceLabel.VerticalAlignment = Microsoft.SPOT.Presentation.VerticalAlignment.Center;
            priceLabel.SetMargin(5, 5, 5, 5);

            this.remainingAmountTextbox = new Text(smallFont, nothingSelectedText);
            this.remainingAmountTextbox.HorizontalAlignment = Microsoft.SPOT.Presentation.HorizontalAlignment.Left;
            this.remainingAmountTextbox.VerticalAlignment = Microsoft.SPOT.Presentation.VerticalAlignment.Center;
            this.remainingAmountTextbox.SetMargin(5, 5, 5, 5);

            Text coinLabel = new Text(smallFont, "Coin:");
            coinLabel.HorizontalAlignment = Microsoft.SPOT.Presentation.HorizontalAlignment.Left;
            coinLabel.VerticalAlignment = Microsoft.SPOT.Presentation.VerticalAlignment.Center;
            coinLabel.SetMargin(5, 5, 5, 5);

            StackPanel leftPanel = new StackPanel(Orientation.Vertical);
            leftPanel.Children.Add(destinationLabel);
            leftPanel.Children.Add(this.productItemsListBox);

            StackPanel centerPanel = new StackPanel(Orientation.Vertical);
            centerPanel.Children.Add(priceLabel);
            centerPanel.Children.Add(this.remainingAmountTextbox);

            StackPanel rightPanel = new StackPanel(Orientation.Vertical);
            rightPanel.Children.Add(coinLabel);
            rightPanel.Children.Add(this.coinSlotsListBox);

            StripPanel topSplit3Pane = new StripPanel(Orientation.Horizontal, new int[] { 2, 2, 1 });
            topSplit3Pane.Children.Add(leftPanel);
            topSplit3Pane.Children.Add(centerPanel);
            topSplit3Pane.Children.Add(rightPanel);

            this.outputSlot = new Text();
            this.outputSlot.Font = smallFont;
            this.outputSlot.TextContent = "Output Slot";
            this.outputSlot.HorizontalAlignment = Microsoft.SPOT.Presentation.HorizontalAlignment.Center;
            this.outputSlot.VerticalAlignment = Microsoft.SPOT.Presentation.VerticalAlignment.Center;
            this.outputSlot.SetMargin(5, 5, 5, 5);

            StripPanel mainSplitHorizontal = new StripPanel(Orientation.Vertical, new int[] { 70, 10, 20 });
            mainSplitHorizontal.Children.Add(topSplit3Pane);
            mainSplitHorizontal.Children.Add(this.outputSlot);
            mainSplitHorizontal.Children.Add(this.infoTextbox);

            this.mainPage = mainSplitHorizontal;

            Text help = new Text();
            help.TextWrap = true;
            help.Font = smallFont;
            help.TextContent = "Welcome to the StaMa vending machine sample.\n" +
                               "\n" +
                               "To start the sample\n" +
                               "    press the SELECT button in the center.\n" +
                               "To change between products\n" +
                               "    press the UP or DOWN buttons.\n" +
                               "To adopt a product for buying\n" +
                               "    press the SELECT button in the center.\n" +
                               "To insert a coin\n" +
                               "    press the SELECT button in the center.\n" +
                               "To change between coins\n" +
                               "    press the UP or DOWN buttons.\n" +
                               "To abort operation\n" +
                               "    press the RIGHT button.\n" +
                               "\n" +
                               "Now press the SELECT button in the center.";
            help.HorizontalAlignment = Microsoft.SPOT.Presentation.HorizontalAlignment.Center;
            help.VerticalAlignment = Microsoft.SPOT.Presentation.VerticalAlignment.Center;
            help.SetMargin(5, 5, 5, 5);

            this.introPage = help;

            this.mainWindow.AddHandler(Buttons.ButtonUpEvent, new RoutedEventHandler(MainWindow_ButtonUp), false);

            this.mainWindow.Visibility = Visibility.Visible;

            Buttons.Focus(this.mainWindow);
        }


        private void SetIntroPage()
        {
            this.mainWindow.Child = this.introPage;
        }


        private void SetMainPage()
        {
            this.mainWindow.Child = this.mainPage;
        }


        private static void AddCoinSlot(ListBox listBox, int coinValue)
        {
            Font smallFont = Resources.GetFont(Resources.FontResources.small);

            HighlightingListBoxItem coinSlot = new HighlightingRoundTextListBoxItem(smallFont, coinValue.ToString());
            coinSlot.SetMargin(5, 5, 5, 5);
            coinSlot.Tag = coinValue;
            listBox.Items.Add(coinSlot);
        }


        private static void AddProductItem(ListBox listBox, string name, int price)
        {
            Font smallFont = Resources.GetFont(Resources.FontResources.small);

            ProductItem productItem = new ProductItem(name, price);
            HighlightingListBoxItem item = new HighlightingTextListBoxItem(smallFont, productItem.Name);
            item.SetMargin(5, 5, 5, 5);
            item.Tag = productItem;
            listBox.Items.Add(item);
        }


        private void MainWindow_ButtonUp(object sender, RoutedEventArgs eventArgs)
        {
            ButtonEventArgs buttonEventArgs = (ButtonEventArgs)eventArgs;
            Button button = buttonEventArgs.Button;
            if ((button == Button.VK_SELECT) ||
                (button == Button.VK_LEFT))
            {
                this.stateMachine.SendTriggerEvent(ButtonSelect);
            }
            else if (button == Button.VK_RIGHT)
            {
                this.stateMachine.SendTriggerEvent(ButtonCancel);
            }
        }


        private void InitStateMachine()
        {
            this.introTimeout = new DispatcherTimer();
            this.introTimeout.Interval = new TimeSpan(0, 0, 30);
            this.introTimeout.Tick += new EventHandler(IntroTimeout_Tick);

            this.processTimeout = new DispatcherTimer();
            this.processTimeout.Interval = new TimeSpan(0, 0, 15);
            this.processTimeout.Tick += new EventHandler(ProcessTimeout_Tick);

            this.deliverTimeout = new DispatcherTimer();
            this.deliverTimeout.Interval = new TimeSpan(0, 0, 5);
            this.deliverTimeout.Tick += new EventHandler(DeliverTimeout_Tick);

            StateMachineTemplate t = new StateMachineTemplate();

            //## Begin StateMachineTemplate
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "TicketVendingNETMF"
            // at 07-22-2015 22:02:47 using StaMaShapes Version 2300
            t.Region(Intro, false);
                t.State(Intro, EnterIntro, ExitIntro);
                    t.Transition("IntroAck", Idle, ButtonSelect, null, null);
                    t.Transition("IntroTimeout", Idle, IntroTimeout, null, null);
                t.EndState();
                t.State(Idle, EnterIdle, ExitIdle);
                    t.Transition("ProductSelected", Empty, ButtonSelect, null, null);
                t.EndState();
                t.State(ItemSelected, EnterItemSelected, ExitItemSelected);
                    t.Region(Empty, false);
                        t.State(Empty, EnterEmpty, ExitEmpty);
                            t.Transition("ClientAbort1", Idle, ButtonCancel, null, null);
                            t.Transition("CoinInserted1", CoinInserted, ButtonSelect, null, null);
                            t.Transition("OperationTimeout1", Idle, ProcessTimeout, null, null);
                        t.EndState();
                        t.State(EmitRequired, null, ExitEmitRequired);
                            t.Region(CoinInserted, false);
                                t.State(CoinInserted, EnterCoinInserted, ExitCoinInserted);
                                    t.Transition("MakeDeal", CloseBargain, null, CheckBalance, null);
                                    t.Transition("CoinInserted2", CoinInserted, ButtonSelect, null, null);
                                    t.Transition("ClientAbort2", Delivering, ButtonCancel, null, null);
                                    t.Transition("OperationTimeout2", Delivering, ProcessTimeout, null, null);
                                t.EndState();
                                t.State(CloseBargain, EnterCloseBargain, null);
                                    t.Transition("Deliver", Delivering, null, null, null);
                                t.EndState();
                            t.EndRegion();
                        t.EndState();
                        t.State(Delivering, EnterDelivering, ExitDelivering);
                            t.Transition("Delivered", Idle, DeliverTimeout, null, null);
                        t.EndState();
                    t.EndRegion();
                t.EndState();
            t.EndRegion();
            //## End StateMachineTemplate

            stateMachine = t.CreateStateMachine(this);
            stateMachine.TraceStateChange = this.TraceStateChange;

            stateMachine.Startup();
        }


        private void TraceStateChange(StateMachine stateMachine, StateConfiguration stateConfigurationFrom, StateConfiguration stateConfigurationTo, Transition transition)
        {
            string textContent = "Current state \"" + stateConfigurationTo.ToString() + "\"" + ((transition != null) ? "\nLast transition \"" + transition.Name + "\"" : String.Empty);
            this.infoTextbox.TextContent = textContent;
        }


        private void IntroTimeout_Tick(object sender, EventArgs e)
        {
            stateMachine.SendTriggerEvent(IntroTimeout);
        }


        private void ProcessTimeout_Tick(object sender, EventArgs e)
        {
            stateMachine.SendTriggerEvent(ProcessTimeout);
        }


        private void DeliverTimeout_Tick(object sender, EventArgs e)
        {
            stateMachine.SendTriggerEvent(DeliverTimeout);
        }


        private bool CheckBalance(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            ProductItem productItem = (ProductItem)((HighlightingListBoxItem)(this.productItemsListBox.SelectedItem)).Tag;

            int receivable = productItem.Price;
            return this.depositBalance >= receivable;
        }


        private void EnterIntro(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            this.introTimeout.Start();

            this.mainWindow.Child = introPage;
        }


        private void ExitIntro(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            this.introTimeout.Stop();

            this.mainWindow.Child = mainPage;
        }


        private void EnterIdle(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            Buttons.Focus(productItemsListBox);
            this.productItemsListBox.SelectedIndex = 0;

            this.outputSlot.TextContent = FormatOutputSlot(-1, null);
        }


        private void ExitIdle(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
        }


        private void EnterItemSelected(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            Buttons.Focus(coinSlotsListBox);
            this.coinSlotsListBox.SelectedIndex = 0;

            ProductItem productItem = (ProductItem)((HighlightingListBoxItem)(this.productItemsListBox.SelectedItem)).Tag;
            remainingAmountTextbox.TextContent = productItem.Price.ToString();
        }


        private void ExitItemSelected(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            this.productItemsListBox.SelectedIndex = -1;
            this.coinSlotsListBox.SelectedIndex = -1;

            remainingAmountTextbox.TextContent = nothingSelectedText;
        }


        private void ExitEmitRequired(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            // Transfer depositBalance and acquired product to output slot.
            this.outputSlot.TextContent = FormatOutputSlot(this.depositBalance, this.acquiredProduct);
            this.acquiredProduct = null;
            this.depositBalance = 0;
        }


        private void EnterCoinInserted(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            ProductItem productItem = (ProductItem)((HighlightingListBoxItem)(this.productItemsListBox.SelectedItem)).Tag;
            int coinValue = (int)((HighlightingListBoxItem)(this.coinSlotsListBox.SelectedItem)).Tag;
            this.depositBalance += coinValue;
            remainingAmountTextbox.TextContent = (productItem.Price - this.depositBalance).ToString();

            this.processTimeout.Start();
        }


        private void ExitCoinInserted(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            this.processTimeout.Stop();
        }


        private void EnterDelivering(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            this.deliverTimeout.Start();

            remainingAmountTextbox.TextContent = nothingSelectedText;
        }


        private void ExitDelivering(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            this.deliverTimeout.Stop();
        }


        private void EnterCloseBargain(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            ProductItem productItem = (ProductItem)((HighlightingListBoxItem)(this.productItemsListBox.SelectedItem)).Tag;

            // Do the transaction.
            this.depositBalance -= productItem.Price;
            this.acquiredProduct = productItem;

            this.coinSlotsListBox.SelectedIndex = -1;
        }


        private void EnterEmpty(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            this.processTimeout.Start();
        }


        private void ExitEmpty(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            this.processTimeout.Stop();
        }


        private static string FormatOutputSlot(int returnBalance, ProductItem productItem)
        {
            string content;
            if (returnBalance < 0)
            {
                content = "              ";
            }
            else
            {
                content = (returnBalance > 0) ? returnBalance.ToString() : String.Empty;
                if (productItem != null)
                {
                    content = String.Concat(content, " \"" + productItem.Name + "\"");
                }
                else
                {
                    content = String.Concat(content, "          ");
                }
            }
            return String.Concat("Output slot: [ ", content, " ]");
        }


        private class ProductItem
        {
            public readonly string Name;
            public readonly int Price;

            public ProductItem(string name, int price)
            {
                this.Name = name;
                this.Price = price;
            }
        }
    }
}
