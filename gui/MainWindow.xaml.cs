using System;
using System.IO.Ports;
using System.Windows;

namespace WpfApp3
{
    public partial class MainWindow : Window
    {
        private SerialPort serialPort; // Оголошення змінної для порту серійного зв'язку

        public MainWindow()
        {
            InitializeComponent(); // Ініціалізація компонентів вікна
            TypeComboBox.Items.Add("Прямокутний сигнал"); // Додавання елемента в комбінований список для типу сигналу
            TypeComboBox.Items.Add("Синус"); // Додавання елемента в комбінований список для типу сигналу
            TypeComboBox.SelectedIndex = 0; // Встановлюємо значення за замовчуванням (Прямокутний сигнал)

            SignalLevlBox.Items.Add("Високий"); // Додавання елемента для рівня сигналу (Високий)
            SignalLevlBox.Items.Add("Низький"); // Додавання елемента для рівня сигналу (Низький)
            SignalLevlBox.SelectedIndex = 0; // Встановлюємо значення за замовчуванням (Високий)
        }

        // Обчислення чексу для періоду, глибини, типу сигналу і рівня сигналу
        private byte CalculateChecksum(long signalType, long signalLevel, long depth, long period)
        {
            long checksum = 0;  // Використовуємо тип long для обробки великих чисел

            // Розбиваємо значення на байти і зберігаємо їх в окремих змінних

            // Для Типу сигналу (1 байт)
            byte signalTypeByte = (byte)(signalType & 0xFF);  // Операція маски для отримання молодшого байта
            // Для Рівня сигналу (1 байт)
            byte signalLevelByte = (byte)(signalLevel & 0xFF);  // Операція маски для отримання молодшого байта

            // Розбиваємо глибину на два байти (молодший і старший)
            byte depthLowByte = (byte)(depth & 0xFF);              // Молодший байт глибини
            byte depthHighByte = (byte)((depth >> 8) & 0xFF);      // Старший байт глибини

            // Розбиваємо період на два байти (молодший і старший)
            byte periodLowByte = (byte)(period & 0xFF);            // Молодший байт періоду
            byte periodHighByte = (byte)((period >> 8) & 0xFF);    // Старший байт періоду

            // Тепер XOR кожного байта по черзі

            checksum ^= signalTypeByte;  // XOR для Типу сигналу
            checksum ^= signalLevelByte; // XOR для Рівня сигналу

            checksum ^= depthLowByte;    // XOR для Молодшого байту глибини
            checksum ^= depthHighByte;   // XOR для Старшого байту глибини

            checksum ^= periodLowByte;   // XOR для Молодшого байту періоду
            checksum ^= periodHighByte;  // XOR для Старшого байту періоду

            // Повертаємо результат чексу, обмежений 1 байтом
            return (byte)(checksum & 0xFF);  // Обмежуємо результат до 1 байта, щоб отримати значення від 0 до 255
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string portName = PortTextBox.Text;  // Отримуємо ім'я порту з текстового поля
            string periodText = PeriodTextBox.Text;  // Отримуємо значення періоду з текстового поля
            string depthText = DetheTextBox.Text;  // Отримуємо значення глибини з текстового поля
            string signalTypeText = TypeComboBox.SelectedItem.ToString();  // Отримуємо вибраний тип сигналу з комбінованого списку
            string signalLevelText = SignalLevlBox.SelectedItem.ToString();  // Отримуємо вибраний рівень сигналу з комбінованого списку

            try
            {
                // Перевірка, чи введено коректні числа для Period і Depth
                if (!long.TryParse(periodText, out long period))
                {
                    MessageBox.Show("Некоректне значення періоду.");  // Якщо значення періоду некоректне, виводимо повідомлення
                    return;
                }

                if (!long.TryParse(depthText, out long depth))
                {
                    MessageBox.Show("Некоректне значення глибини.");  // Якщо значення глибини некоректне, виводимо повідомлення
                    return;
                }

                // Перевірка на наявність правильного типу сигналу
                long signalType = 0;
                if (signalTypeText == "Прямокутний сигнал")
                {
                    signalType = 1; // Прямокутний сигнал
                }
                else if (signalTypeText == "Синус")
                {
                    signalType = 2; // Синус
                }
                else
                {
                    MessageBox.Show("Некоректний тип сигналу.");  // Якщо тип сигналу некоректний, виводимо повідомлення
                    return;
                }

                // Перевірка на наявність правильного рівня сигналу
                long signalLevel = 0;
                if (signalLevelText == "Високий")
                {
                    signalLevel = 2; // Високий рівень
                }
                else if (signalLevelText == "Низький")
                {
                    signalLevel = 1; // Низький рівень
                }
                else
                {
                    MessageBox.Show("Некоректний рівень сигналу.");  // Якщо рівень сигналу некоректний, виводимо повідомлення
                    return;
                }

                // Перевірка на наявність відкритого порту
                if (serialPort == null || !serialPort.IsOpen)
                {
                    serialPort = new SerialPort(portName, 115400, Parity.None, 8, StopBits.One);  // Відкриваємо порт з відповідними налаштуваннями
                    serialPort.Open();  // Встановлюємо з'єднання з портом
                    MessageBox.Show("Підключення встановлено.");  // Виводимо повідомлення про успішне підключення
                }

                // Обчислюємо чек суму
                byte checksum = CalculateChecksum(signalType, signalLevel, depth, period);  // Викликаємо метод для обчислення чексу

                // Виводимо дані у новому порядку
                MessageBox.Show($"Дані відправлено:\nТип сигналу = {signalType}\nРівень сигналу = {signalLevel}\nГлибина = {depth}\nПерiод = {period}\nЧек сума: {checksum.ToString()}");  // Виводимо результат на екран
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");  // Виводимо повідомлення про помилку, якщо щось пішло не так
            }
        }

        private void DetheTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Обробник змін у текстовому полі для глибини (якщо потрібно)
        }

        private void PortTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Обробник змін у полі порту (якщо потрібно)
        }

        private void TypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Обробник зміни типу даних (якщо потрібно)
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Робота для Антона :) \n[ASCII арт]");  // Виведення повідомлення при натисканні на кнопку "Info"
        }
    }
}
