# 1 Develop and document communication protocol
Communication between a personal computer and the STM32F051DISCOVERY microcontroller takes place via UART. It allows you to set signal parameters and receive information about the current state of the generator.The hardware is implemented using an additional USB/UART-converter. The structural and functional diagram is shown in Figure 1.

![image](https://github.com/user-attachments/assets/0d6041c3-9234-4133-a727-80c1f78fe9a0)
>                      Figure 1 - Structural and functional diagram

UART-Parameters: 
* Transmission speed: 115200 baud
* Frame format: 8 data bits, 1 stop bit, no parity
* Data format: Binary (HEX) + CRC8

Table 1 - Hardware connection between STM32F051DISCOVERY and USB/UART-converter
| STM32F051DISCOVERY | USB/UART-converter |
|-------------|-------------|
|   5V PIN    |     VCC     | 
|    GND      |     GND     |
| PA10 (UART1-TX) |  TXD  |
|  PA9 (UART1-RX) |  RXD  |

## 1.1 Description of the protocol (secret information)
