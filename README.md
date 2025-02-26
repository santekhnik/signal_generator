# 1 Develop and document communication protocol
Communication between a personal computer and the STM32F051DISCOVERY microcontroller takes place via UART. The hardware is implemented using an additional USB/UART converter. The structural and functional diagram is shown in Figure 1.

![image](https://github.com/user-attachments/assets/87e82d2d-3c19-425e-bd1e-8634da0142ac)     
>                      Figure 1 - Structural and functional diagram



Table 1 -- Сonnection UART
| Заголовок 1 | Заголовок 2 | Заголовок 3 |
|-------------|-------------|-------------|
| Ячейка 1   | Ячейка 2   | Ячейка 3   |
| Ячейка 4   | Ячейка 5   | Ячейка 6   |

## 1.1 Description of the protocol (secret information)
The protocol is used to exchange data between the STM32F0Discovery and an external device via UART. It allows you to set signal parameters and receive information about the current state of the generator.
Transport layer: UART (RS-232 / TTL) 
Transmission speed: 115200 baud
Frame format: 8 data bits, 1 stop bit, no parity
Data format: Binary (HEX) + CRC8
