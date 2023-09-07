#include <LiquidCrystal_I2C.h>
#include <Wire.h>

LiquidCrystal_I2C lcd(0x27, 16, 2);

String inData;
int switchDelay = 1500;

const uint8_t up_sign[] = {
  0b00100,
  0b01110,
  0b10101,
  0b00100,
  0b00100,
  0b00100,
  0b00100,
  0b00100,
};

const uint8_t down_sign[] = {
  0b00100,
  0b00100,
  0b00100,
  0b00100,
  0b00100,
  0b10101,
  0b01110,
  0b00100,
};

const uint8_t deg_sign[] = {
  0b00110,
  0b01001,
  0b01001,
  0b00110,
  0b00000,
  0b00000,
  0b00000,
  0b00000,
};

void setup() {
  lcd.init();
  Serial.begin(9600);
  lcd.backlight();

  lcd.createChar(1, up_sign);
  lcd.createChar(2, down_sign);
  lcd.createChar(3, deg_sign);

  displayMessage();
}

bool connected = false;

void loop() {

  String cpuUsage = "";  //!
  String gpuUsage = "";  //@
  String cpuTemp = "";   //#
  String gpuTemp = "";   //$
  String upSpeed = "";   //%
  String dwSpeed = "";   //^

  while (Serial.available() > 0) {
    char recieved = Serial.read();
    inData += recieved;

    if (inData.length() > 0) {

      if (recieved == '~') {
        lcd.clear();
        connected = false;
      } else
        connected = true;

      if (recieved == '!') {
        inData.remove(inData.length() - 1, 1);
        cpuUsage = inData;
        inData = "";
      }

      if (recieved == '@') {
        inData.remove(inData.length() - 1, 1);
        gpuUsage = inData;
        inData = "";
      }

      if (recieved == '#') {
        inData.remove(inData.length() - 1, 1);
        cpuTemp = inData;
        inData = "";
      }

      if (recieved == '$') {
        inData.remove(inData.length() - 1, 1);
        gpuTemp = inData;
        inData = "";
      }

      if (recieved == '%') {
        inData.remove(inData.length() - 1, 1);
        upSpeed = inData;
        inData = "";
      }

      if (recieved == '^') {
        inData.remove(inData.length() - 1, 1);
        dwSpeed = inData;
        inData = "";
      }
    }
  }

  if (!connected) {
    displayMessage();
  } else {
    // if (state == 0) {
      displayData1(cpuUsage, cpuTemp, gpuUsage, gpuTemp);
    // } else {
      displayData2(upSpeed, dwSpeed);
    // }    
  }
}

void displayMessage() {
  lcd.setCursor(0, 0);
  lcd.print("READY_TO_CONNECT_");
  lcd.setCursor(0, 1);
  lcd.print("____KAZIRUS_____");

  delay(2000);

  lcd.setCursor(0, 0);
  lcd.print("Open the desktop");
  lcd.setCursor(0, 1);
  lcd.print("client 2 connect");

  delay(2000);
}

void displayData1(String cU, String cT, String gU, String gT) {
  lcd.clear();

  lcd.setCursor(0, 0);
  lcd.print("CPU: " + cU + "% " + cT);
  lcd.print((char)0x03);
  lcd.print("C");

  lcd.setCursor(0, 1);
  lcd.print("GPU: " + gU + "% " + gT);
  lcd.print((char)0x03);
  lcd.print("C");

  lcd.display();
  delay(switchDelay);
}

void displayData2(String up, String down) {
  lcd.clear();

  lcd.setCursor(0, 0);
  lcd.print((char)0x01);
  lcd.print(" ");
  lcd.print(up);

  lcd.setCursor(0, 1);
  lcd.print((char)0x02);
  lcd.print(" ");
  lcd.print(down);

  lcd.display();
  delay(switchDelay);
}