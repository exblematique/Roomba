#include <cstdio>
#include <cstring>
#include <chrono>
#include <thread>

#include "command.h"
#include "OpenInterfaceConfig.h"
#include "lib/SerialLink/SerialLink.h"

using namespace std::chrono_literals;

command::command()
    : running{false}
    , sl{"/dev/ttyUSB0", static_cast<unsigned int>(Baud::ROOMBA_DEFAULT)}
{
    sl.write(startSafe());
}

command::~command()
{
}

// Roomba run until it receive the stop signal
const void command::run(int16_t speed){
    changeVelocity(speed, speed);
    while (running);
    changeVelocity(0, 0);
}

void command::square(int16_t length) {
    for (int i=0; i<4; i++) {
        sl.write(changeVelocity(-150, 150));
        std::this_thread::sleep_for(3s);
        sl.write(changeVelocity(100, 100));
        std::this_thread::sleep_for(std::chrono::seconds(length));
    }
    sl.write(changeVelocity(0, 0));
    changeState();
}

const void command::sensor(int16_t speed){
    changeVelocity(speed, speed);
    while (running) {
    // Add sensor part on this part    
    ;}
    changeVelocity(0, 0);
}

void command::stop(){
    changeState();
}

const bool command::getState() {return running;}
void command::changeState() {running = not running;}