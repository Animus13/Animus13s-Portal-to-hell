using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PatientMonitor
{
    class PatientMonitoringController
    {
        readonly MainWindow _mainWindow = null;
        readonly IPatientFactory _patientFactory = null;
        DispatcherTimer _tickTimer = new DispatcherTimer();
        PatientDataReader _dataReader;
        PatientData _patientData;
        PatientAlarmer _alarmer;

        CheckBox _alarmMuter;
        Label _pulseRate;
        Label _breathingRate;
        Label _systolicPressure;
        Label _diastolicPressure;
        Label _temperature;

        public PatientMonitoringController(MainWindow window, IPatientFactory patientFactory)
        {
            _patientFactory = patientFactory;
            _mainWindow = window;
            _pulseRate = _mainWindow.pulseRate1;
            _breathingRate = _mainWindow.breathingRate1;
            _systolicPressure = _mainWindow.systolic1;
            _diastolicPressure = _mainWindow.diastolic1;
            _temperature = _mainWindow.temperature1;
            _alarmMuter = _mainWindow.AlarmMute1;
               
            
        }

        public void RunMonitor()
        {
            setupComponents();
            setupUI();
        }

        void setupUI()
        {
            //_mainWindow.patientSelector.SelectionChanged
            //    += new System.Windows.Controls.SelectionChangedEventHandler(newPatientSelected);
            newPatientSelected();

            _mainWindow.heartRateLower1.AlarmValue = (int)DefaultSettings.LOWER_PULSE_RATE;
            _mainWindow.breathingRateLower1.AlarmValue = (int)DefaultSettings.LOWER_BREATHING_RATE;
            _mainWindow.temperatureLower1.AlarmValue = (int)DefaultSettings.LOWER_TEMPERATURE;
            _mainWindow.systolicLower1.AlarmValue = (int)DefaultSettings.LOWER_SYSTOLIC;
            _mainWindow.diastolicLower1.AlarmValue = (int)DefaultSettings.LOWER_DIASTOLIC;

            _mainWindow.heartRateUpper1.AlarmValue = (int)DefaultSettings.UPPER_PULSE_RATE;
            _mainWindow.breathingRateUpper1.AlarmValue = (int)DefaultSettings.UPPER_BREATHING_RATE;
            _mainWindow.temperatureUpper1.AlarmValue = (int)DefaultSettings.UPPER_TEMPERATURE;
            _mainWindow.systolicUpper1.AlarmValue = (int)DefaultSettings.UPPER_SYSTOLIC;
            _mainWindow.diastolicUpper1.AlarmValue = (int)DefaultSettings.UPPER_DIASTOLIC;

            _mainWindow.heartRateLower1.ValueChanged += new EventHandler(limitsChanged);
            _mainWindow.breathingRateLower1.ValueChanged += new EventHandler(limitsChanged);
            _mainWindow.temperatureLower1.ValueChanged += new EventHandler(limitsChanged);
            _mainWindow.systolicLower1.ValueChanged += new EventHandler(limitsChanged);
            _mainWindow.diastolicLower1.ValueChanged += new EventHandler(limitsChanged);

            _mainWindow.heartRateUpper1.ValueChanged += new EventHandler(limitsChanged);
            _mainWindow.breathingRateUpper1.ValueChanged += new EventHandler(limitsChanged);
            _mainWindow.temperatureUpper1.ValueChanged += new EventHandler(limitsChanged);
            _mainWindow.systolicUpper1.ValueChanged += new EventHandler(limitsChanged);
            _mainWindow.diastolicUpper1.ValueChanged += new EventHandler(limitsChanged);
        }


        void limitsChanged(object sender, EventArgs e)
        {
            _alarmer.PulseRateTester.LowerLimit = _mainWindow.heartRateLower1.AlarmValue;
            _alarmer.BreathingRateTester.LowerLimit = _mainWindow.breathingRateLower1.AlarmValue;
            _alarmer.TemperatureTester.LowerLimit = _mainWindow.temperatureLower1.AlarmValue;
            _alarmer.SystolicBpTester.LowerLimit = _mainWindow.systolicLower1.AlarmValue;
            _alarmer.DiastolicBpTester.LowerLimit = _mainWindow.diastolicLower1.AlarmValue;

            _alarmer.PulseRateTester.UpperLimit = _mainWindow.heartRateUpper1.AlarmValue;
            _alarmer.BreathingRateTester.UpperLimit = _mainWindow.breathingRateUpper1.AlarmValue;
            _alarmer.TemperatureTester.UpperLimit = _mainWindow.temperatureUpper1.AlarmValue;
            _alarmer.SystolicBpTester.UpperLimit = _mainWindow.systolicUpper1.AlarmValue;
            _alarmer.DiastolicBpTester.UpperLimit = _mainWindow.diastolicUpper1.AlarmValue;
        }

        void setupComponents()
        {
            _patientData = (PatientData)_patientFactory.CreateandReturnObj(PatientClassesEnumeration.PatientData);
            _dataReader = (PatientDataReader)_patientFactory.CreateandReturnObj(PatientClassesEnumeration.PatientDataReader);
            _alarmer = (PatientAlarmer)_patientFactory.CreateandReturnObj(PatientClassesEnumeration.PatientAlarmer);

            _alarmer.BreathingRateAlarm += new EventHandler(soundMutableAlarm);
            _alarmer.DiastolicBloodPressureAlarm += new EventHandler(soundMutableAlarm);
            _alarmer.PulseRateAlarm += new EventHandler(soundMutableAlarm);
            _alarmer.SystolicBloodPressureAlarm += new EventHandler(soundMutableAlarm);
            _alarmer.TemperatureAlarm += new EventHandler(soundMutableAlarm);
            _tickTimer.Stop();
            _tickTimer.Interval= TimeSpan.FromMilliseconds(1000);
            _tickTimer.Tick += new EventHandler(updateReadings);
        }

        void updateReadings(object sender, EventArgs e)
        {            
            _patientData.SetPatientData(_dataReader.getData());
            _pulseRate.Content = _patientData.PulseRate;
            _breathingRate.Content = _patientData.BreathingRate;
            _systolicPressure.Content = _patientData.SystolicBloodPressure;
            _diastolicPressure.Content = _patientData.DiastolicBloodPressure;
            _temperature.Content = _patientData.Temperature;
            _alarmer.ReadingsTest(_patientData);
        }

        void newPatientSelected()
        {
            _tickTimer.Stop();
            string fileName = @"..\..\..\Bed 1.csv";
            _dataReader.Connect(fileName);
            _tickTimer.Start();
        }

        void soundMutableAlarm(object sender, EventArgs e)
        {
            if(_alarmMuter.IsChecked == false)
            {
                _mainWindow.soundMutableAlarm();
            }
        }
    }
}
