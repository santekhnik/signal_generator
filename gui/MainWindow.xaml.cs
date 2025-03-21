using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp3
{
    public partial class MainWindow : Window
    {
        private SerialPort serialPort;

        public MainWindow()
        {
            InitializeComponent();
            TypeComboBox.Items.Add("PWM");
            TypeComboBox.Items.Add("SIN");
            TypeComboBox.Items.Add("TRI");
            TypeComboBox.SelectedIndex = 0;

            SignalLevlBox.Items.Add("HIGH");
            SignalLevlBox.Items.Add("LOW");
            SignalLevlBox.SelectedIndex = 0;

            this.Closing += MainWindow_Closing;

        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string portName = PortTextBox.Text;
            string periodText = PeriodTextBox.Text;
            string depthText = DetheTextBox.Text;
            string signalTypeText = TypeComboBox.SelectedItem.ToString();
            string signalLevelText = SignalLevlBox.SelectedItem.ToString();

            try
            {

                if (serialPort == null || serialPort.PortName != portName)
                {
                    CloseSerialPort(); // Закриваємо попередній порт перед відкриттям нового
                    serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
                }

                // Відкриваємо порт, якщо він ще не відкритий
                if (!serialPort.IsOpen)
                {
                    serialPort.DataReceived += SerialPort_DataReceived;
                    serialPort.Open();
                }


                if (string.IsNullOrWhiteSpace(depthText) && string.IsNullOrWhiteSpace(periodText))
                {
                    MessageBox.Show("Both depth and period values are missing.");
                    return;
                }

                // Перевіряємо, чи введені значення є числами
                bool isDepthValid = uint.TryParse(depthText, out uint depth);
                bool isPeriodValid = uint.TryParse(periodText, out uint period);

                // Перевіряємо вихід за межі
                bool isDepthOutOfRange = isDepthValid && (depth < 0 || depth > 65535);
                bool isPeriodOutOfRange = isPeriodValid && (period < 0 || period > 65535);

                if (isDepthOutOfRange && isPeriodOutOfRange)
                {
                    MessageBox.Show("Both depth and period values are out of range (0 μs to 65535 μs).");
                    return;
                }

                if (!isDepthValid || isDepthOutOfRange)
                {
                    if (!string.IsNullOrWhiteSpace(depthText) && !depthText.All(char.IsDigit))
                    {
                        MessageBox.Show("Depth value contains invalid characters. Only numbers are allowed.");
                    }
                    else if (isDepthOutOfRange)
                    {
                        MessageBox.Show("Incorrect depth value (Value can be from 0 μs to 65535 μs).");
                    }
                    return;
                }

                if (!isPeriodValid || isPeriodOutOfRange)
                {
                    if (!string.IsNullOrWhiteSpace(periodText) && !periodText.All(char.IsDigit))
                    {
                        MessageBox.Show("Period value contains invalid characters. Only numbers are allowed.");
                    }
                    else if (isPeriodOutOfRange)
                    {
                        MessageBox.Show("Incorrect period value (Value can be from  μs to 65535 μs).");
                    }
                    return;
                }


                // Вибір типу сигналу 
                byte signalType;
                switch (signalTypeText)
                {
                    case "PWM":
                        signalType = 1; // Генерація ПРямокутного сигналу 
                        break;
                    case "SIN":
                        signalType = 2; // Синус
                        break;
                    case "TRI":
                        signalType = 3; // Трикутник
                        break;
                    default:
                        signalType = 1; // ДефолтПрямокутний
                        break;
                }

                byte signalLevel = signalLevelText == "LOW" ? (byte)1 : (byte)2;

                if (serialPort == null)
                {
                    serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
                }

                if (!serialPort.IsOpen)
                {
                    serialPort.DataReceived += SerialPort_DataReceived; // Додаємо тільки один раз
                    serialPort.Open();
                }

                byte[] commandPacket = new byte[]
                {
                     signalType,
                     signalLevel,

                     (byte)(depth ), (byte)((depth >> 8) & 0xFF),(byte)((depth >> 16) & 0xFF),(byte)((depth >> 24) & 0xFF),
                     (byte)(period ), (byte)((period >> 8) & 0xFF) , (byte)((period >> 16) & 0xFF),(byte)((period >> 24) & 0xFF),
                };

                byte checksum = CalculateChecksum.CalculateChecksum_Function(signalType, signalLevel, depth, period);

                byte[] fullPacket = new byte[commandPacket.Length + 2];
                Array.Copy(commandPacket, fullPacket, commandPacket.Length);

                fullPacket[fullPacket.Length - 2] = checksum;

                SendToDevice(fullPacket);

                //OutputTextBox.Text += $"\nДані відправлено:\nТип сигналу = {signalType}\nРівень сигналу = {signalLevel}\nГлибина = {depth}\nПеріод = {period}\nЧек сума: {checksum}";
                OutputTextBox.ScrollToEnd();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }



        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var serialPort = (SerialPort)sender;

                if (serialPort == null || !serialPort.IsOpen)
                    return; // Перевіряємо, чи порт відкритий перед читанням
                while (serialPort.BytesToRead > 0)
                {
                    int b = serialPort.ReadByte();
                    Dispatcher.Invoke(() =>
                    {
                        if (b == 0x031) // Якщо отримано 1 (0x01 у HEX)
                        {
                            OutputTextBox.Text += "\n📢 Start of signal generation !";
                        }
                        else if (b == 0x032) // Якщо отримано 2 (0x02 у HEX)
                        {
                            OutputTextBox.Text += "\n⚠️ The checksum did not match !";
                        }
                        /*
                        else
                        {
                            OutputTextBox.Text += $"\nОтримано байт: {b:X2} (Dec: {b})";
                        }
                        */
                        OutputTextBox.ScrollToEnd();
                        OutputTextBox.Text += $" \n---- ---- ---- ---- ";

                    });

                }
            }

            catch (IOException ex)
            {
                if (serialPort != null && serialPort.IsOpen) // Запобігаємо обробці, якщо порт уже закритий
                {
                    Dispatcher.Invoke(() =>
                    {
                        OutputTextBox.Text += "\nReception error (IO): " + ex.Message;
                    });
                }
            }
            catch (InvalidOperationException)
            {
                Dispatcher.Invoke(() =>
                {
                    OutputTextBox.Text += "\nReceive error: port closed.";
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    OutputTextBox.Text += "\nReception error: " + ex.Message;
                });
            }
        }



        private void SendToDevice(byte[] data)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write(data, 0, data.Length);
            }
        }

        private void CloseSerialPort()
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.DataReceived -= SerialPort_DataReceived;  // Видаляємо обробник
                    serialPort.Close();
                }
                serialPort.Dispose();
                serialPort = null;
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseSerialPort(); // Закриваємо порт перед виходом
        }



        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            if (TypeComboBox.SelectedItem != null)
            {
                string selectedType = TypeComboBox.SelectedItem.ToString();
                bool isPwm = selectedType == "PWM";

                SignalLevlPanel.Visibility = isPwm ? Visibility.Visible : Visibility.Collapsed;
                DetheTextPanel.Visibility = isPwm ? Visibility.Visible : Visibility.Collapsed;

                // Якщо вибрано не "PWM", встановлюємо глибину в 0
                if (!isPwm)
                {
                    DetheTextBox.Text = "0";
                }

                // PeriodPanel залишається видимим незалежно від вибору
            }
        }

        private void PortTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Логіка обробки зміни тексту в полі вводу
        }

        private void ScanPort(object sender, RoutedEventArgs e)
        {

            try
            {
                string[] ports = SerialPort.GetPortNames(); // Отримуємо список доступних COM-портів

                if (ports.Length > 0)
                {
                    PortTextBox.Text = ports[0]; // Встановлюємо перший знайдений порт у TextBox
                }
                else
                {
                    PortTextBox.Text = ""; // Очищаємо поле, якщо портів немає
                    MessageBox.Show("No COM port found!", "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while scanning ports: {ex.Message}",
                                "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(" Folk Signal Generator \n Version 3.0.1 \n Created by CtrlAltDelete Team: \n Ivan Bortsikh and Taras Bilyk \n Build platform: 64-bit x86 Windows\n Compiler: emulating Visual Studio 2022\n © 2024. All rights reserved.\n Folk Signal Generator is a program for configuring a signal generator with a period range from 1 µs to 1 s.The interface allows users to select signal parameters and send them to the device via a serial port.The generator supports sine, triangular, and square waveforms.\n How to Use:\n 1 Launching the Program\n Open Folk Signal Generator.Ensure the generator is connected to the computer via the appropriate COM port.\n 2 Selecting Signal Parameters:\n 2.1 Type: Choose the signal shape (sine, triangular, or square).\n 2.2 Port: Select the COM port used for data transmission.\n 2.3 Signal Level: Set the signal level (low or high).\n 2.4 Depth: Enter the desired signal depth.\n 2.5 Period: Specify the signal generation period(from 1 µs to 1 s).\n 3 Sending Configuration:\n Click the Send button to transmit the settings to the generator.The lower field will display the execution status and received responses.\n 4 Error Diagnostics:\n 4.1 If the status field displays \"ERROR,\" check the cable connection or port.\n 4.2 If signal generation does not work, ensure the parameters are within allowable values and the COM port is open.\n 5 Verifying Operation:\n If the generator successfully receives the command, the program will display a response (e.g., \"Received byte: 06F\").\n 6 Closing the Program:\n When finished, click the \"X\" button in the upper right corner to close the window.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DetheTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Логіка обробки зміни тексту
        }
    }
}
