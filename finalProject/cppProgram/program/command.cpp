#include <cstdio>
#include <cstring>
#include <chrono>
#include <thread>

#include "command.h"
#include "../lib/SerialLink/SerialLink.h"
#include "OpenInterfaceConfig.h"

using namespace std::chrono_literals;

/**
 *  \brief command::command starts a communication with an external device at a certain baud rate
 *  \param mqtt mq_ is used to send stop signal
 **/
command::command()
    : mq{new mqttStop("SignalStop", "localhost", 1883)}
{
    sl->write(oic->startSafe());
}

/**
 *  \brief command::command~ delete the current object
 *  \note This function is not used
 **/
command::~command()
{
}

/**
 *  \brief command::run move forword the Roomba until running variable become false
 *  \param int16_t speed define the speed for both wheel
 *  \note This function stopped when running variable become false.
 **/
// Roomba run until it receive the stop signal
const void command::run(int16_t speed){
    running = true;
    sl->write(oic->changeVelocity(speed, speed));
    while (running);
    sl->write(oic->changeVelocity(0, 0));
}

/**
 *  \brief command::square move forward the Roomba until running variable become false
 *  \param int16_t length define the size of square side
 *  \note This function can be interrupted when running variable become false. 
 **/
void command::square(int16_t length) {
    running = true;
    std::chrono::system_clock::time_point time;
    for (int i=0; i<4; i++) {
        //Turning phase
        time = std::chrono::system_clock::now();
        sl->write(oic->changeVelocity(-150, 150));
        while (std::chrono::system_clock::now() - time < 3s){
            if (!running){
                sl->write(oic->changeVelocity(0, 0));
                return;
            }
        }
        
        //Move forward phase
        time = std::chrono::system_clock::now();
        sl->write(oic->changeVelocity(100, 100));
        while (std::chrono::system_clock::now() - time < std::chrono::seconds(length)){
            if (!running){
                sl->write(oic->changeVelocity(0, 0));
                return;
            }
        }
    }
    sl->write(oic->changeVelocity(0, 0));
    running = false;
}

/**
 *  \brief command::sensor move forward the Roomba and when a wall is detected the roomba turn. 
 *  \param int16_t speed define the speed for both wheel
 *  \note This function stopped when running variable become false.
 **/
const void command::sensor(int16_t speed){
    running = true;
    sl->write(oic->changeVelocity(speed, speed));
    std::vector<uint8_t> dt{};
    while (running) {
        sl->write(oic->SENSORS);
        std::this_thread::sleep_for(2s);
        for(int i = 0; i < 10; i++){
            dt = sl->read(2);
            sl->write(oic->SENSORS);
        }
        if(dt==oic->WALL_ALL||dt==oic->WALL_RIGHT||dt==oic->WALL_LEFT){
            sl->write(oic->changeVelocity(-200, -200));
            std::this_thread::sleep_for(1s);
            sl->write(oic->changeVelocity( 0, -200));
            std::this_thread::sleep_for(1s);
      	    sl->write(oic->changeVelocity(200, 200));
        }
    }
    sl->write(oic->changeVelocity(0, 0));
}

/**
 *  \brief command::stop change the running variable to false
 **/
void command::stop(){
    running = false;
    mq->sendStop();
}

/**
 *  \brief command::getState return the value of running variable
 **/
const bool command::getState() {return running;}