## Signal Generator
---

## [Technical Task](technical_task.md)

---

# 1. Overview

This document describes the communication protocol for configuring and controlling the pulse generator based on STM32. The protocol defines the structure of commands sent from a PC to the STM32 via UART and the responses sent back by the STM32.

# 2. [Envoriment Setup](env_setup.md)

# 3 Develop and document communication protocol 

## 3.1 Communication Protocol Description

Communication between the STM32F051R8T6 microcontroller and a personal computer will be carried out using asynchronous UART/USART transmission. This allows setting signal parameters and obtaining information about the current state of the generator.

The structural diagram of data transmission is shown in Figure 3.1.
![image](https://github.com/user-attachments/assets/bb0f9d32-b3bd-48e2-90ae-bb6ef52ec5ab)
Figure 3.1 – Structural Diagram of Communication between the STM32F051R8T6 Microcontroller and the Personal Computer

Since the STM32F051 Discovery Board does not have a built-in USB/UART converter, an external module is used to establish a connection between the microcontroller and the personal computer, as depicted in Figure 1.1.

On the personal computer, a user interface window will display input/output data and a virtual COM port connection window.

## 3.2 Message and Data Packet Structure

To generate rectangular pulses, a command is sent from the user terminal, which is converted into a binary or hexadecimal code combination and transmitted to the STM32F051R8T6 microcontroller via the UART protocol.

The proposed message format is as follows:

█'N''S' 'x' 'y' 'check'

Where:

N – Signal number (since the project is designed for possible expansion to support different signal forms, each form will have its own sequential number). In the current setup, only one type of signal is used, assigned number 1.

S – Initial state level of the rectangular signal (1 for high level, 0 for low level).

x – Duration of the low-level period (in microseconds).

y – Duration of the high-level period (in microseconds).

check – Checksum, calculated as the sum of all previous parameters. This parameter is computed separately in both the user interface software and the STM32F051R8T6 microcontroller firmware.

Example of a Command:

█('1' '0' '1000' '500' '1501')

Based on this command, the microcontroller will generate a rectangular pulse sequence:

The first signal is formed.

The sequence starts with a low level lasting 1000 µs.

Followed by a high level lasting 500 µs.

Thus, the period of this signal is x + y, and the duty cycle is given by the ratio y / (x + y).

The waveform corresponding to command (1.2) is shown in Figure 1.2.
![image](https://github.com/user-attachments/assets/e10348c9-cf76-4e91-acbc-06e967b4b08b)
Figure 1.2 – Rectangular Signal Generation for the Given Command

The microcontroller receives this command as a packet of hexadecimal messages (e.g., 0x01 0x00 0x00 for generating a PWM signal). Upon successful reception, the microcontroller responds with a confirmation message such as:

Signal generate successful

This communication protocol ensures reliable and structured data exchange between the user interface and the STM32F051R8T6 microcontroller for generating precise pulse sequences.

Expanding error messages for detailed diagnostics.

Supporting additional waveform types.

This protocol ensures structured communication between the PC and STM32 for precise control of pulse generation.

## 3.3  The program's response to possible errors

Let us consider cases of errors in command formation or incorrect reception of this code combination on the microcontroller. It should be noted that the user will not be able to enter this command. The command is formed on the input device in the user interface program code. The user himself will be able to enter the pulse depth and signal period parameters. Hence, there may be a case when the user enters the value of the pulse depth parameter greater than the period. Then the PWM depth is greater than 1, which is impossible, since the duration of the high level to the entire period must be less than 1. When the program sees this, an error is displayed on the user panel in the form of a text message, for example, “ERROR CONFIGURATION”.

Another error that will be displayed when transmitting a command (data packet) under the influence of high interference. For this, the <check> parameter was formed. This parameter, as mentioned above, can be represented as the sum of the previous parameters. But it would be useful to add checked bits to this packet, which would make the data packet noise-proof. Such checking is carried out by entering cyclic codes or a code with parity check. Thus, the <check> parameter will be calculated in the user interface, after which it will be calculated independently in the program for the microcontroller and in this program there will be a function for comparing these parameters. The command will be considered accepted correctly if the values ​​of the check parameters in the interface and the microcontroller match. Otherwise, the microcontroller returns the message “ERROR UART RECEIVE/TRANSMIT”.

If an error occurs during signal transmission via the UART protocol, the structure of the code combination is violated, which affects the result of the sum. The parameters will be summed byte by byte using the logical functions of disjunction, conjunction, exclusive “OR”.

# 4 CONFIGURING THE HARDWARE PART OF THE PROJECT

The scope of this task is to configure the external hardware and set up the environment for the hardware part. Using STM32CubeMX within the STM32CubeIDE program, create a project for the STM32F051DiscoveryBoard board, including such peripherals as UART configuration.

## 4.1 External hardware part of the project

As mentioned in the first section, the STM32F051DiscoveryBoard debugging board does not have the ability to transfer data via the UART protocol via the USB port. Based on this, it is necessary to connect a device to the microcontroller board that will perform the functions of a USB/UART converter. The one shown in Figure 4.1 was selected.
![image](https://github.com/user-attachments/assets/9b6462ce-2482-4936-8d50-430b4d0d9f35)
Figure 4.1 – Appearance of the FT232RL USB/UART converter board

The STM32F051R8T6 microcontroller has two pairs of asynchronous transfer ports. For this project, the first pair (so-called USART_1) was selected, which are ports PA9 for uart_tx (transmit) and PA10 for uart_rx (receive). The connection diagram of these boards is shown in Figure 4.2 (made in Splan).
![image](https://github.com/user-attachments/assets/acea84d1-dbf7-4669-a433-731b9e88400c)
Figure 4.2 – Connection diagram of the STM32F051DiscoveryBoard microcontroller to the USB/UART converter board

The diagram also includes connecting the LED to the microcontroller. The STM32F051DiscoveryBoard board already contains this LED, which is connected through a resistor with a nominal value of 330 Ohms. The LED is needed to provide indication of the operation of the generator channel.
For the presentability of the project, it was decided to make a custom printed circuit board. The printed circuit board design was made in the EasyEDAPro environment. The layout of the board's gerber file is shown in Figure 2.3.
![image](https://github.com/user-attachments/assets/48663069-ac8c-4265-826c-9c84f1683b49)
Figure 4.3 – PCB layout for a presentable layout

## 4.2 Configuring the internal hardware of the project
The internal hardware of the project consists of configuring the STM32CubeIDE software environment. After creating the project (File → New → STM32 Project → select the STM32F051R8T6 microcontroller and the C programming language).
The general system hardware configuration is performed (the “System Core” tab in the “Pinout & Configuration” window). In the SYS tab, Debug Serial Wire is selected, and in the RCC tab, timer settings. The modes are selected, as shown in Figure 2.4.
![image](https://github.com/user-attachments/assets/326e8e6e-6da4-4fbb-a1d8-c02ca4961038)
Figure 4.4 – Configuring the “Pinout & Configuration” tab (screenshot from the STM32CubeIDE software environment)

Go to the “Connectivity” tab, where the USART_1 parameters are configured. As mentioned above, the operating mode is asynchronous, the transmission speed is 115200 Baud (bit/s). The speed is set in the “Baud Rate” window of the “Parameter Settings” tab. Also, it is necessary to enable the global interrupt mode (NVIC Settings tab).
Go to the GPIO tab in the System Core window and provide your own names for the UART ports. If configured correctly, the virtual microcontroller in the software environment will look like the one shown in Figure 2.5.

![image](https://github.com/user-attachments/assets/c2c474c7-18dd-42ef-87fa-f3016e25c703)
Figure 4.5 – Appearance of the microcontroller in the software environment (screenshot)

Go to the “Clock Configuration” tab, where the ceramic resonator is configured. For the STM32F051R8T6 microcontroller, in the “PLL Sourse Mux” block, the lower position “HSE” is selected and the divider “/1” is set in front of it. After that, in the “*PLLMul” block, the multiplier “×6” and the divider “/1” are set, thereby setting the port output to a frequency of 72 MHz.
In the “Sysem Clock Mux” block, the lower position “PLLCLK” is set. Next, the frequency is automatically set to 48 MHz, the divider “/1”. Thus, in the “APB1 Peripheral clock” block, the frequency should be set to 48 MHz. The result of the setting is shown in Figure 4.6.

![image](https://github.com/user-attachments/assets/4e577d26-f1ec-4ac9-ad12-fe702a9d5681)
Figure 4.6 – Setting the “Clock Configuration” tab

Thus, the preliminary setup of the internal hardware of the project is completed. As of now, this is not the final setup. They are provided in the following sections if necessary. Further research may include optimizing the settings to reduce power consumption, adding support for additional peripherals, and improving the methods for debugging the UART connection.

## 4.3 Formulation of conclusions 

The section includes the configuration of the hardware part of the project, which includes external hardware connections and settings of the internal parameters of the microcontroller in the STM32CubeIDE environment.
1) The implemented hardware connections allow the effective use of the STM32F051R8T6 microcontroller as part of the hardware-software complex. The UART interface for data exchange via the FT232RL converter is configured, which ensures stable transmission and reception of information. The internal configuration of the microcontroller corresponds to modern approaches to the development of embedded systems.
2) The implemented hardware and software settings can be used in embedded systems that require control via a serial interface. This includes automated control systems, IoT solutions and industrial controllers.
3) In the process of work, a printed circuit board for a presentable device layout was designed. Using STM32CubeIDE allows you to quickly configure the parameters of the microcontroller

# 5 IMPLEMENTING A DRIVER FOR UART TRANSMISSION

The purpose of this task is to create 2 functions. The first one is for sending bytes via UART, example API: UART_Send(uint16_t byte_count, uint32_t *data_buffer_ptr). This function should send a certain number of bytes from the selected buffer via UART. The second function is for receiving bytes, example API: UART_Receive(uint16_t byte_count, uint32_t *data_buffer_ptr). This function should receive a certain number of bytes and store them in the buffer. To make sure that everything works, for example, you can connect the RX and TX pins on the stm32f051 board and send data via the TX pin and receive it on the RX pin.

## 5.1 Initialization of functions for receiving and transmitting messages

After configuring the hardware part of the project, it is necessary to programmatically initialize the function algorithms for UART data reception and transmission. To do this, prototypes of two API functions are initialized, the first for receiving data (Receive), the second for transmitting (Transmit). The initialization is given below between the corresponding comments.

/* USER CODE BEGIN PFP */

void UART_Transmit(uint16_t byte_count, uint8_t *data_buffer_ptr);
void UART_Receive(uint16_t byte_count, uint8_t *data_buffer_ptr);

/* USER CODE END PFP */

The HAL library has initialized functions for receiving and transmitting messages. The contents of the initialized function prototypes are given below. As can be seen from the excerpt of the program text (program code), it is necessary to indicate which pair of ports is used for UART/USART. The parameters that these functions accept are necessarily indicated.
/* USER CODE BEGIN 0 */

void UART_Transmit (uint16_t byte_count, uint8_t *data_buffer_ptr)
{
HAL_UART_Transmit(&huart1, data_buffer_ptr, byte_count, HAL_MAX_DELAY);
}

void UART_Receive(uint16_t byte_count, uint8_t *data_buffer_ptr)
{
HAL_UART_Receive(&huart1, data_buffer_ptr, byte_count, HAL_MAX_DELAY);
}

/* USER CODE END 0 */

It is noted that the initialization of the above functions is only a structure (rule) according to which these functions should be used when formulating methods. An example for checking the correctness of the configuration of these functions is given in 5.2.

## 5.2 Algorithm and writing the program text for UART data transmission

Let there be a data array uint8_t array_message[5]= {1,2,3,4,5}. From the computer terminal (Real_Term) an array_buffer [SIZE_OF_ARRAY] is input, which has the same width as the array_message array (i.e. SIZE_OF_ARRAY = 5). If the elements of the input array (array_buffer) coincide with the message array (array_message), then the word “Ok” is output to the terminal, if not – “Error”.
The implementation of this algorithm consists in checking the elements of the input and given arrays, that is, element by element. Depending on the result, a word is output, indicating the correctness of the input. This is done by calling the UART_Transmit () function. The input of numbers (in this case, it is a byte message) occurs by calling the UART_Receive() function.
The block diagram of the algorithm of this program is shown in Figure 5.1. As can be seen from the block diagram, the entire procedure.
![image](https://github.com/user-attachments/assets/ad0020ba-9add-4cef-ac64-4af2897e5af9)
Figure 5.1 – Block diagram of the algorithm of the program for checking the entered array

In the text file of the main function, the initialization of variables and other data types is implemented in the corresponding blocks, and in an infinite loop, the array data is checked in accordance with the block diagram shown in Figure 3.1.
It is necessary to connect two main libraries for the C language and for working with strings, as well as initialize a variable that will be responsible for the size of the array

/* Private includes -----------------------------------------------------*/
/* USER CODE BEGIN Includes */
#include <stdio.h>
#include <string.h>
/* USER CODE END Includes */

/* Private define -------------------------------------------------------*/
/* USER CODE BEGIN PD */
#define SIZE_OF_ARRAY (5)
/* USER CODE END PD */

Initialization of arrays in the main program file, in particular arrays for outputting messages to the terminal

/* USER CODE BEGIN PV */
uint8_t array_message[SIZE_OF_ARRAY] = {1,2,3,4,5};
uint8_t array_buffer [SIZE_OF_ARRAY];
uint8_t receive [] = {'O','k'};
uint8_t error_receive [SIZE_OF_ARRAY] = {'E','r','r','o','r'};
/* USER CODE END PV */

After initializing all the necessary libraries, variables and arrays, the verification program is given in an infinite loop. The result of the program execution is shown in Figure 3.2.

/* Infinite loop */
/* USER CODE BEGIN WHILE */
while (1)
{
UART_Receive(sizeof(array_buffer), array_buffer);

uint8_t match = 1;
for (int i = 0; i < SIZE_OF_ARRAY; i++)
{
if (array_buffer[i] != array_message[i])
{ match = 0; break; }
}

if (match) { UART_Transmit(sizeof(receive), receive); }
else { UART_Transmit(sizeof(error_receive), error_receive); }
/* USER CODE END WHILE */

/* USER CODE BEGIN 3 */
}
/* USER CODE END 3 */
}

![image](https://github.com/user-attachments/assets/99f0326e-3d1b-454f-b5b6-1b660668a857)
Figure 5.2 – Program test results (screenshot from RealTerm program)

As can be seen from Figure 5.2, when entering the correct array (first line), the program returns the word "Ok", when entering the wrong one (second line)

# 6 CHECKSUM IMPLEMENTATION METHOD

The purpose of this task is to develop a C function to calculate the checksum. Since it was not decided during the planning which algorithm to use, the scope of the task is also to decide whether to use BCC (Block Check Character), CRC (Cyclic Redundancy Check), or any other. That is, it is necessary to analyze different solutions and choose the one that will satisfy the needs of the algorithms and the capabilities of the developers to implement the corresponding algorithms.

# 6.1 Analysis of different methods of noise-resistant coding

The checksum is used to verify the integrity of data in transmitted or stored messages. There are several basic methods for calculating the checksum: BCC (Block Check Character), CRC (Cyclic Redundancy Check), and other algorithms, such as simple byte addition. Below we will consider their features, advantages, and disadvantages.
The BCC code uses a bitwise XOR operation to calculate the checksum of a data block. This is a simple and fast method suitable for basic error checking.
CRC uses polynomial arithmetic to calculate the checksum. It is much more reliable than BCC and is widely used in telecommunication protocols.
A simple way to get the checksum is to add all the bytes together and take the low byte of the sum. Therefore, this checksum method was chosen to start with, since BCC (XOR) was chosen for its simplicity and speed. It is suitable for detecting simple errors, but does not provide a high level of protection. If a more reliable mechanism is needed, CRC is a better option. Simply adding bytes can also be used in simple scenarios.
Example of BCC encoding in C

#include <stdio.h>

unsigned char calculate_bcc(const unsigned char* data, size_t length)
{
unsigned char checksum = 0;
for (size_t i = 0; i < length; i++) { checksum ^= data[i]; }
return checksum;
}

int main() {
unsigned char data[] = { 0x12, 0x34, 0x56, 0x78 };
unsigned char checksum = calculate_bcc(data, sizeof(data));
printf("BCC Checksum: 0x%X\n", checksum);
return 0;
}

Thus, the BCC (XOR) code is chosen because of its simplicity and speed. It is suitable for detecting simple errors, but does not provide a high level of protection. If a more reliable mechanism is required, CRC is a better option. Simple addition of bytes can also be used in simple scenarios.

# 6.2 Formulation of the checksum verification algorithm

To build the checksum verification algorithm, the UART_Data () function is created, in which the checksum of the given numbers will be checked. The program algorithm is shown in Figure 6.1. The program also contains a check for the number of entered numbers in the array. If the number of numbers is more than 5, the program sends the word Error to the terminal. After checking for the number of elements, the program processes these numbers and sums them modulo 2 (the “exceptional or” operation). This process occurs in a loop. The function call is performed in an infinite loop of the main program function.

![image](https://github.com/user-attachments/assets/b64bbcae-a0ca-4c06-b6ef-9edc5f07fb14)
Figure 6.1 – Block diagram of the algorithm of the checksum calculation program

Writing the text of the UART_Data () program in the STM32CubeIDE programming environment looks like this.

void UART_Data()
{
uint8_t Data_Numbers[SIZE_OF_ARRAY] = {0}; // Cleared buffer
HAL_StatusTypeDef status;

uint8_t first_byte; // Waiting for the first byte (blocks the program until there is no input)
status = HAL_UART_Receive(&huart1, &first_byte, 1, HAL_MAX_DELAY);
if (status != HAL_OK)
{ UART_Transmit(strlen((char*)Error_Receive), Error_Receive); return; }

Data_Numbers[0] = first_byte; // Get the remaining 4 bytes (since the first one is already there)
status = HAL_UART_Receive(&huart1, &Data_Numbers[1], SIZE_OF_ARRAY - 1, 100);
if (status != HAL_OK)
{UART_Transmit(strlen((char*)Error_Receive), Error_Receive); return; }

uint8_t extra_byte; // Check for extra bytes (if more than 5 were entered)
if (HAL_UART_Receive(&huart1, &extra_byte, 1, 10) == HAL_OK)
{ UART_Transmit(strlen((char*)Error_Receive), Error_Receive); return; }

uint8_t xorsum = 0; // If exactly 5 bytes, calculate XOR
for (int i = 0; i < SIZE_OF_ARRAY; i++) { xorsum ^= Data_Numbers[i]; }
char output[10]; snprintf(output, sizeof(output), "0x%02X", xorsum);
UART_Transmit(strlen(output), (uint8_t*)output);
}

This function is called in an infinite loop. The check is performed using the RealTerm terminal program. The result of the program performance check is shown in Figure 6.2.

![image](https://github.com/user-attachments/assets/f0f57cb3-f8fa-42b8-848b-3ac5e8542b36)
Figure 6.2 – Result of the program execution (screenshot from the RealTerm terminal)

As can be seen from Figure 4.2, the program calculates the sum of numbers modulo 2 if the number of numbers is five, if more or less – the program will give an error.

## 6.3 Formulation of conclusions for the fourth section
During the work, an analysis was conducted of various data integrity control methods, in particular BCC (block check character), CRC (cyclic redundancy check) and simple p
