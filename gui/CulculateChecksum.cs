using System;
using System.Text;


namespace WpfApp3
{
    static class CalculateChecksum
    {
        // Обчислення чексу для періоду, глибини, типу сигналу і рівня сигналу
        public static byte CalculateChecksum_Function(long signalType, long signalLevel, long depth, long period)
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
            byte depthHighByte1 = (byte)((depth >> 16) & 0xFF);
            byte depthHighByte2 = (byte)((depth >> 24) & 0xFF);
            // Розбиваємо період на два байти (молодший і старший)
            byte periodLowByte = (byte)(period & 0xFF);            // Молодший байт періоду
            byte periodHighByte = (byte)((period >> 8) & 0xFF);
            byte periodHighByte1 = (byte)((period >> 16) & 0xFF);
            byte periodHighByte2 = (byte)((period >> 24) & 0xFF);// Старший байт періоду

            // Тепер XOR кожного байта по черзі

            checksum ^= signalTypeByte;  // XOR для Типу сигналу
            checksum ^= signalLevelByte; // XOR для Рівня сигналу

            checksum ^= depthLowByte;    // XOR для Молодшого байту глибини
            checksum ^= depthHighByte;   // XOR для Старшого байту глибини
            checksum ^= depthHighByte1;
            checksum ^= depthHighByte2;

            checksum ^= periodLowByte;   // XOR для Молодшого байту періоду
            checksum ^= periodHighByte;  // XOR для Старшого байту періоду
            checksum ^= periodHighByte1;
            checksum ^= periodHighByte2;

            // Повертаємо результат чексу, обмежений 1 байтом
            return (byte)(checksum & 0xFF);  // Обмежуємо результат до 1 байта, щоб отримати значення від 0 до 255
        }
    }
}
