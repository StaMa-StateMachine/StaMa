using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using StaMa;

namespace Clock
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class DlgMain : System.Windows.Forms.Form
	{
        //## Begin StateAndTransitionNames
        // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "Clock Sample"
        // at 07-22-2015 22:02:43 using StaMaShapes Version 2300
        private const string Operating= "Operating";
        private const string AlarmDisplay= "AlarmDisplay";
        private const string AlarmDisplayAcknowledge= "AlarmDisplayAcknowledge";
        private const string AlarmDisplayTimeout= "AlarmDisplayTimeout";
        private const string NormalDisplay= "NormalDisplay";
        private const string AlarmTimeNow= "AlarmTimeNow";
        private const string AlarmMode= "AlarmMode";
        private const string ChangeAlarmEnable= "ChangeAlarmEnable";
        private const string ChangeAlarmToggleAlarm= "ChangeAlarmToggleAlarm";
        private const string GoChangeAlarmHour= "GoChangeAlarmHour";
        private const string RevertDisplayAlarm3= "RevertDisplayAlarm3";
        private const string ChangeAlarmEnableTimeout= "ChangeAlarmEnableTimeout";
        private const string ChangeAlarmHour= "ChangeAlarmHour";
        private const string ChangeAlarmHourUp= "ChangeAlarmHourUp";
        private const string GoChangeAlarmMinutes= "GoChangeAlarmMinutes";
        private const string RevertDisplayAlarm1= "RevertDisplayAlarm1";
        private const string ChangeAlarmHourTimeout= "ChangeAlarmHourTimeout";
        private const string ChangeAlarmMinutes= "ChangeAlarmMinutes";
        private const string ChangeAlarmMinutesUp= "ChangeAlarmMinutesUp";
        private const string GoToggleAlarm= "GoToggleAlarm";
        private const string RevertDisplayAlarm2= "RevertDisplayAlarm2";
        private const string ChangeAlarmMinutesTimeout= "ChangeAlarmMinutesTimeout";
        private const string InspectAlarm= "InspectAlarm";
        private const string GotoStopwatchMode= "GotoStopwatchMode";
        private const string GoChangeAlarm= "GoChangeAlarm";
        private const string InspectAlarmTimeout= "InspectAlarmTimeout";
        private const string DateMode= "DateMode";
        private const string GotoAlarmMode= "GotoAlarmMode";
        private const string DateModeTimeout= "DateModeTimeout";
        private const string StopwatchMode= "StopwatchMode";
        private const string GotoTimeMode= "GotoTimeMode";
        private const string StopwatchModeReset= "StopwatchModeReset";
        private const string StopwatchModeStart= "StopwatchModeStart";
        private const string StopwatchModeStop= "StopwatchModeStop";
        private const string TimeMode= "TimeMode";
        private const string GotoDateMode= "GotoDateMode";
        private const string StopwatchRunning= "StopwatchRunning";
        private const string StopwatchStop= "StopwatchStop";
        private const string StopwatchStopped= "StopwatchStopped";
        private const string StopwatchStart= "StopwatchStart";
        private const string StopwatchReset= "StopwatchReset";
        private const string AlarmActive= "AlarmActive";
        private const string DeactivateAlarm= "DeactivateAlarm";
        private const string AlarmDeactivated= "AlarmDeactivated";
        private const string ActivateAlarm= "ActivateAlarm";
        //## End StateAndTransitionNames


        private enum EventId
        {
            Mode,
            Set,
            Up,
            Stop,
            Start,
            Reset,
            ActivateAlarm,
            DeactivateAlarm,
            AlarmTimeNow,
            DateModeTimeout,
            InspectAlarmTimeout,
            ChangeAlarmHourTimeout,
            ChangeAlarmMinutesTimeout,
            ChangeAlarmEnableTimeout,
            AlarmDisplayTimeout,
        }


        private const int TICK_DELTA = 10;
        private const int TIMEOUT = 15000;
        private DateTime m_stopWatchStarted = new DateTime(0);
        private TimeSpan m_stopWatchElapsed = new TimeSpan(0);
        private DateTime m_alarmTime = new DateTime(0);

        private System.Windows.Forms.Label m_wndDisplay;
        private System.Windows.Forms.Button m_wndSet;
        private System.Windows.Forms.Button m_wndUp;
        private System.Windows.Forms.Button m_wndMode;
        private System.Windows.Forms.Timer m_timer;
        private System.ComponentModel.IContainer components;

        private static StateConfiguration m_stateShowTime;
        private static StateConfiguration m_stateShowDate;
        private static StateConfiguration m_stateShowAlarm;
        private static StateConfiguration m_stateAlarmActive;
        private static StateConfiguration m_stateShowStopwatch;
        private static StateConfiguration m_stateStopwatchRunning;
        private static StateConfiguration m_stateAlarmDisplay;

        private System.Windows.Forms.Label m_wndActiveState;
        private StateMachine m_stateMachine = null;
        private System.Windows.Forms.CheckBox m_wndAlarmActive;
        private System.Windows.Forms.Button m_wndClose;
        private IDictionary m_timers;
        private bool m_flicker;

		public DlgMain()
		{
            StateMachineTemplate t = new StateMachineTemplate();

            //## Begin StateMachineTemplate
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "Clock Sample"
            // at 07-22-2015 22:02:44 using StaMaShapes Version 2300
            t.Region(Operating, false);
                t.State(Operating, null, null);
                    t.Region(NormalDisplay, false);
                        t.State(AlarmDisplay, EnterAlarmDisplay, ExitAlarmDisplay);
                            t.Transition(AlarmDisplayAcknowledge, NormalDisplay, EventId.Up, null, null);
                            t.Transition(AlarmDisplayTimeout, NormalDisplay, EventId.AlarmDisplayTimeout, null, null);
                        t.EndState();
                        t.State(NormalDisplay, null, null);
                            t.Transition(AlarmTimeNow, new string[] {AlarmActive, NormalDisplay}, AlarmDisplay, EventId.AlarmTimeNow, null, null);
                            t.Region(TimeMode, true);
                                t.State(AlarmMode, null, null);
                                    t.Region(InspectAlarm, false);
                                        t.State(ChangeAlarmEnable, EnterChangeAlarmEnable, ExitChangeAlarmEnable);
                                            t.Transition(ChangeAlarmToggleAlarm, ChangeAlarmEnable, EventId.Up, null, ToggleAlarm);
                                            t.Transition(GoChangeAlarmHour, ChangeAlarmHour, EventId.Set, null, null);
                                            t.Transition(RevertDisplayAlarm3, InspectAlarm, EventId.Mode, null, null);
                                            t.Transition(ChangeAlarmEnableTimeout, InspectAlarm, EventId.ChangeAlarmEnableTimeout, null, null);
                                        t.EndState();
                                        t.State(ChangeAlarmHour, EnterChangeAlarmHour, ExitChangeAlarmHour);
                                            t.Transition(ChangeAlarmHourUp, ChangeAlarmHour, EventId.Up, null, IncrementAlarmHour);
                                            t.Transition(GoChangeAlarmMinutes, ChangeAlarmMinutes, EventId.Set, null, null);
                                            t.Transition(RevertDisplayAlarm1, InspectAlarm, EventId.Mode, null, null);
                                            t.Transition(ChangeAlarmHourTimeout, InspectAlarm, EventId.ChangeAlarmHourTimeout, null, null);
                                        t.EndState();
                                        t.State(ChangeAlarmMinutes, EnterChangeAlarmMinutes, ExitChangeAlarmMinutes);
                                            t.Transition(ChangeAlarmMinutesUp, ChangeAlarmMinutes, EventId.Up, null, IncrementAlarmMinutes);
                                            t.Transition(GoToggleAlarm, ChangeAlarmEnable, EventId.Set, null, null);
                                            t.Transition(RevertDisplayAlarm2, InspectAlarm, EventId.Mode, null, null);
                                            t.Transition(ChangeAlarmMinutesTimeout, InspectAlarm, EventId.ChangeAlarmMinutesTimeout, null, null);
                                        t.EndState();
                                        t.State(InspectAlarm, EnterInspectAlarm, ExitInspectAlarm);
                                            t.Transition(GotoStopwatchMode, StopwatchMode, EventId.Mode, null, null);
                                            t.Transition(GoChangeAlarm, ChangeAlarmHour, EventId.Set, null, null);
                                            t.Transition(InspectAlarmTimeout, TimeMode, EventId.InspectAlarmTimeout, null, null);
                                        t.EndState();
                                    t.EndRegion();
                                t.EndState();
                                t.State(DateMode, EnterDateMode, ExitDateMode);
                                    t.Transition(GotoAlarmMode, InspectAlarm, EventId.Mode, null, null);
                                    t.Transition(DateModeTimeout, TimeMode, EventId.DateModeTimeout, null, null);
                                t.EndState();
                                t.State(StopwatchMode, null, null);
                                    t.Transition(GotoTimeMode, TimeMode, EventId.Mode, null, null);
                                    t.Transition(StopwatchModeReset, new string[] {StopwatchMode, StopwatchStopped}, StopwatchMode, EventId.Set, null, TriggerReset);
                                    t.Transition(StopwatchModeStart, new string[] {StopwatchMode, StopwatchStopped}, StopwatchMode, EventId.Up, null, TriggerStart);
                                    t.Transition(StopwatchModeStop, new string[] {StopwatchMode, StopwatchRunning}, StopwatchMode, EventId.Up, null, TriggerStop);
                                t.EndState();
                                t.State(TimeMode, null, null);
                                    t.Transition(GotoDateMode, DateMode, EventId.Mode, null, null);
                                t.EndState();
                            t.EndRegion();
                        t.EndState();
                    t.EndRegion();
                    t.Region(StopwatchStopped, false);
                        t.State(StopwatchRunning, null, null);
                            t.Transition(StopwatchStop, StopwatchStopped, EventId.Stop, null, StopwatchDoStop);
                        t.EndState();
                        t.State(StopwatchStopped, null, null);
                            t.Transition(StopwatchStart, StopwatchRunning, EventId.Start, null, StopwatchDoStart);
                            t.Transition(StopwatchReset, StopwatchStopped, EventId.Reset, null, StopwatchDoReset);
                        t.EndState();
                    t.EndRegion();
                    t.Region(AlarmDeactivated, false);
                        t.State(AlarmActive, null, null);
                            t.Transition(DeactivateAlarm, AlarmDeactivated, EventId.DeactivateAlarm, null, null);
                        t.EndState();
                        t.State(AlarmDeactivated, null, null);
                            t.Transition(ActivateAlarm, AlarmActive, EventId.ActivateAlarm, null, null);
                        t.EndState();
                    t.EndRegion();
                t.EndState();
            t.EndRegion();
            //## End StateMachineTemplate


            m_stateShowTime = t.CreateStateConfiguration(TimeMode);
            m_stateShowDate = t.CreateStateConfiguration(DateMode);
            m_stateShowAlarm = t.CreateStateConfiguration(AlarmMode);
            m_stateAlarmActive = t.CreateStateConfiguration(AlarmActive);
            m_stateShowStopwatch = t.CreateStateConfiguration(StopwatchMode);
            m_stateStopwatchRunning = t.CreateStateConfiguration(StopwatchRunning);
            m_stateAlarmDisplay = t.CreateStateConfiguration(AlarmDisplay);


            m_alarmTime = m_alarmTime.AddHours((24 + DateTime.Now.Hour - 2) % 24);
            m_alarmTime = m_alarmTime.AddMinutes(DateTime.Now.Minute);

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            m_timer.Interval = TICK_DELTA;

            EventId[] timerEvents = new EventId[] { EventId.DateModeTimeout,
                                                    EventId.InspectAlarmTimeout,
                                                    EventId.ChangeAlarmHourTimeout,
                                                    EventId.ChangeAlarmMinutesTimeout,
                                                    EventId.ChangeAlarmEnableTimeout,
                                                    EventId.AlarmDisplayTimeout };
            m_timers = new ListDictionary();
            foreach (EventId timerEvent in timerEvents)
            {
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer(this.components);
                m_timers.Add(timerEvent, timer);
                timer.Tag = timerEvent;
                timer.Tick += new EventHandler(this.m_timer_Timeout);
                timer.Interval = TIMEOUT;
            }

            m_stateMachine = t.CreateStateMachine(this);

            m_stateMachine.TraceTestTransition = StateMachine_TraceTestTransition;
            m_stateMachine.TraceDispatchTriggerEvent = StateMachine_TraceDispatchEvent;

            m_stateMachine.Startup();
            m_wndActiveState.Text = m_stateMachine.ActiveStateConfiguration.ToString();
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
                    m_stateMachine.Finish();
                    components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.m_wndDisplay = new System.Windows.Forms.Label();
            this.m_wndSet = new System.Windows.Forms.Button();
            this.m_wndUp = new System.Windows.Forms.Button();
            this.m_wndMode = new System.Windows.Forms.Button();
            this.m_timer = new System.Windows.Forms.Timer(this.components);
            this.m_wndActiveState = new System.Windows.Forms.Label();
            this.m_wndAlarmActive = new System.Windows.Forms.CheckBox();
            this.m_wndClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_wndDisplay
            // 
            this.m_wndDisplay.Font = new System.Drawing.Font("Lucida Console", 20F);
            this.m_wndDisplay.Location = new System.Drawing.Point(152, 64);
            this.m_wndDisplay.Name = "m_wndDisplay";
            this.m_wndDisplay.Size = new System.Drawing.Size(177, 37);
            this.m_wndDisplay.TabIndex = 0;
            this.m_wndDisplay.Text = "88:88:88";
            this.m_wndDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_wndSet
            // 
            this.m_wndSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_wndSet.Location = new System.Drawing.Point(344, 40);
            this.m_wndSet.Name = "m_wndSet";
            this.m_wndSet.Size = new System.Drawing.Size(48, 23);
            this.m_wndSet.TabIndex = 6;
            this.m_wndSet.Text = "Set";
            this.m_wndSet.Click += new System.EventHandler(this.m_wndSet_Click);
            // 
            // m_wndUp
            // 
            this.m_wndUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_wndUp.Location = new System.Drawing.Point(344, 96);
            this.m_wndUp.Name = "m_wndUp";
            this.m_wndUp.Size = new System.Drawing.Size(48, 23);
            this.m_wndUp.TabIndex = 5;
            this.m_wndUp.Text = "Up";
            this.m_wndUp.Click += new System.EventHandler(this.m_wndUp_Click);
            // 
            // m_wndMode
            // 
            this.m_wndMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_wndMode.Location = new System.Drawing.Point(88, 96);
            this.m_wndMode.Name = "m_wndMode";
            this.m_wndMode.Size = new System.Drawing.Size(48, 23);
            this.m_wndMode.TabIndex = 4;
            this.m_wndMode.Text = "Mode";
            this.m_wndMode.Click += new System.EventHandler(this.m_wndMode_Click);
            // 
            // m_timer
            // 
            this.m_timer.Enabled = true;
            this.m_timer.Tick += new System.EventHandler(this.m_timer_Tick);
            // 
            // m_wndActiveState
            // 
            this.m_wndActiveState.Location = new System.Drawing.Point(8, 152);
            this.m_wndActiveState.Name = "m_wndActiveState";
            this.m_wndActiveState.Size = new System.Drawing.Size(560, 32);
            this.m_wndActiveState.TabIndex = 1;
            this.m_wndActiveState.Text = "##############";
            this.m_wndActiveState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_wndAlarmActive
            // 
            this.m_wndAlarmActive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.m_wndAlarmActive.Enabled = false;
            this.m_wndAlarmActive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_wndAlarmActive.Location = new System.Drawing.Point(80, 40);
            this.m_wndAlarmActive.Name = "m_wndAlarmActive";
            this.m_wndAlarmActive.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.m_wndAlarmActive.Size = new System.Drawing.Size(56, 24);
            this.m_wndAlarmActive.TabIndex = 2;
            this.m_wndAlarmActive.Text = "Alarm";
            this.m_wndAlarmActive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_wndClose
            // 
            this.m_wndClose.Location = new System.Drawing.Point(480, 192);
            this.m_wndClose.Name = "m_wndClose";
            this.m_wndClose.TabIndex = 3;
            this.m_wndClose.Text = "Close";
            this.m_wndClose.Click += new System.EventHandler(this.m_wndClose_Click);
            // 
            // DlgMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 224);
            this.Controls.Add(this.m_wndClose);
            this.Controls.Add(this.m_wndAlarmActive);
            this.Controls.Add(this.m_wndActiveState);
            this.Controls.Add(this.m_wndMode);
            this.Controls.Add(this.m_wndUp);
            this.Controls.Add(this.m_wndSet);
            this.Controls.Add(this.m_wndDisplay);
            this.Name = "DlgMain";
            this.Text = "Clock";
            this.ResumeLayout(false);

        }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.Run(new DlgMain());
		}

        private void m_wndMode_Click(object sender, System.EventArgs e)
        {
            this.SendEvent(EventId.Mode);
        }

        private void m_wndSet_Click(object sender, System.EventArgs e)
        {
            this.SendEvent(EventId.Set);
        }

        private void m_wndUp_Click(object sender, System.EventArgs e)
        {
            this.SendEvent(EventId.Up);
        }

        private void UpdateDisplay()
        {
            m_wndDisplay.ForeColor = Color.Black;

            if (m_stateMachine.IsInState(m_stateShowTime))
            {
                m_wndDisplay.Text = DateTime.Now.ToString("HH:mm:ss");
            }
            else if (m_stateMachine.IsInState(m_stateShowDate))
            {
                m_wndDisplay.Text = DateTime.Now.ToString("dd-MM-yy");
            }
            else if (m_stateMachine.IsInState(m_stateShowAlarm))
            {
                m_wndDisplay.Text = m_alarmTime.ToString("HH:mm");
            }
            else if (m_stateMachine.IsInState(m_stateShowStopwatch))
            {
                DateTime t = new DateTime(m_stopWatchElapsed.Ticks);
                m_wndDisplay.Text = t.ToString("mm:ss.ff");
            }
            else if (m_stateMachine.IsInState(m_stateAlarmDisplay))
            {
                m_wndDisplay.Text = m_alarmTime.ToString("HH:mm");
                if (m_flicker)
                {
                    m_wndDisplay.ForeColor = Color.Red;
                }
                m_flicker = ! m_flicker;
            }

            if (m_stateMachine.IsInState(m_stateAlarmActive))
            {
                m_wndAlarmActive.Checked = true;
            }
            else
            {
                m_wndAlarmActive.Checked = false;
            }
        }


        private void m_timer_Timeout(object sender, System.EventArgs e)
        {
            Timer timer = (Timer)sender;
            EventId eventId = (EventId)timer.Tag;
            switch (eventId)
            {
                case EventId.DateModeTimeout:
                case EventId.InspectAlarmTimeout:
                case EventId.ChangeAlarmHourTimeout:
                case EventId.ChangeAlarmMinutesTimeout:
                case EventId.ChangeAlarmEnableTimeout:
                case EventId.AlarmDisplayTimeout:
                    this.SendEvent(eventId);
                    break;
            }
        }


        private void SendEvent(EventId eventId)
        {
            int steps = m_stateMachine.SendTriggerEvent(eventId);
            if (steps > 0)
            {
                m_wndActiveState.Text = m_stateMachine.ActiveStateConfiguration.ToString();
            }
        }


        private void m_timer_Tick(object sender, System.EventArgs e)
        {
            if (m_stateMachine.IsInState(m_stateStopwatchRunning))
            {
                m_stopWatchElapsed = DateTime.Now - m_stopWatchStarted;
            }
            
            if (m_stateMachine.IsInState(m_stateAlarmActive))
            {
                if ((m_alarmTime.Hour == DateTime.Now.Hour) &&
                    (m_alarmTime.Minute == DateTime.Now.Minute) &&
                    (DateTime.Now.Second == 0))
                {
                    this.SendEvent(EventId.AlarmTimeNow);
                }
            }
            
            UpdateDisplay();
        }


        private void EnterInspectAlarm(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            ((Timer)m_timers[EventId.InspectAlarmTimeout]).Enabled = true;
        }

        
        private void ExitInspectAlarm(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            ((Timer)m_timers[EventId.InspectAlarmTimeout]).Enabled = false;
        }

        
        private void EnterDateMode(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    ((Timer)m_timers[EventId.DateModeTimeout]).Enabled = true;
        }

        
        private void ExitDateMode(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    ((Timer)m_timers[EventId.DateModeTimeout]).Enabled = false;
        }


        private void TriggerStart(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    this.SendEvent(EventId.Start);
        }


        private void TriggerStop(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    this.SendEvent(EventId.Stop);
        }


        private void TriggerReset(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    this.SendEvent(EventId.Reset);
        }


        private void EnterChangeAlarmHour(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    ((Timer)m_timers[EventId.ChangeAlarmHourTimeout]).Enabled = true;
        }


        private void ExitChangeAlarmHour(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    ((Timer)m_timers[EventId.ChangeAlarmHourTimeout]).Enabled = false;
        }


        private void EnterChangeAlarmMinutes(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    ((Timer)m_timers[EventId.ChangeAlarmMinutesTimeout]).Enabled = true;
        }


        private void ExitChangeAlarmMinutes(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    ((Timer)m_timers[EventId.ChangeAlarmMinutesTimeout]).Enabled = false;
        }


        private void StopwatchDoStart(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    m_stopWatchStarted = DateTime.Now - m_stopWatchElapsed;
                    m_stopWatchElapsed = new TimeSpan(0);
        }


        private void StopwatchDoStop(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
        }


        private void StopwatchDoReset(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    m_stopWatchElapsed = new TimeSpan(0);
        }


        private void IncrementAlarmHour(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    m_alarmTime = m_alarmTime.AddHours(1F);
        }


        private void IncrementAlarmMinutes(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    m_alarmTime = m_alarmTime.AddMinutes(1F);
        }


        private void EnterChangeAlarmEnable(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    ((Timer)m_timers[EventId.ChangeAlarmEnableTimeout]).Enabled = true;
        }


        private void ExitChangeAlarmEnable(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    ((Timer)m_timers[EventId.ChangeAlarmEnableTimeout]).Enabled = false;
        }


        private void EnterAlarmDisplay(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    ((Timer)m_timers[EventId.AlarmDisplayTimeout]).Enabled = true;
        }

        
        private void ExitAlarmDisplay(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    ((Timer)m_timers[EventId.AlarmDisplayTimeout]).Enabled = false;
        }

        
        private void ToggleAlarm(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
                    EventId ev = m_stateMachine.IsInState(m_stateAlarmActive) ? EventId.DeactivateAlarm : EventId.ActivateAlarm;
                    this.SendEvent(ev);
        }


        private void StateMachine_TraceTestTransition(StateMachine stateMachine, Transition transition, object triggerEvent, EventArgs eventArgs)
        {
            String message = String.Format("Test transition {0} with event {1} in state {2}",
                                           transition.ToString(),
                                           (triggerEvent != null) ? triggerEvent.ToString() : "*",
                                           stateMachine.ActiveStateConfiguration.ToString());
            System.Diagnostics.Trace.WriteLine(message);
        }

        private void StateMachine_TraceDispatchEvent(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            String message = String.Format("Dispatch event {0} in state {1}",
                                           (triggerEvent != null) ? triggerEvent.ToString() : "*",
                                           stateMachine.ActiveStateConfiguration.ToString());
            System.Diagnostics.Trace.WriteLine(message);
        }

        private void m_wndClose_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }
    }
}
