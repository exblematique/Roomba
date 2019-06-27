#ifndef OPENINTERFACECONFIG_H
#define OPENINTERFACECONFIG_H

#include <cstdint>
#include <vector>

enum class Baud : unsigned int { ROOMBA_SLOW = 19200, ROOMBA_DEFAULT = 115200 };
const bool FOREVER{true};

// Get started
const uint8_t START{128};
const uint8_t BAUD_1{129};

// Mode commands
const uint8_t MODE_SAFE{131};
const uint8_t MODE_FULL{132};

// Commands
const uint8_t DRIVE_DIRECT_4{145};

std::vector<uint8_t> SENSORS{142, 7};
std::vector<uint8_t> WALL_RIGHT{19, 1};
std::vector<uint8_t> WALL_LEFT{19, 2};
std::vector<uint8_t> WALL_ALL{19, 3};


/**
 *  \brief startSafe initializes the roomba
 *  \return std::vector<uint8_t> which contains all numbers to send to Roomba
 **/
std::vector<uint8_t> startSafe()
{
   return {START, MODE_SAFE};
}

/**
 *  \brief changeVelocity return serial syntax to define the new velocity 
 *  \param rightVelocity is the speed of right wheel with a signed number
 *  \param leftVelocity is the speed of left wheel with a signed number
 *  \return std::vector<uint8_t> which contains all numbers to send to Roomba
 **/
std::vector<uint8_t> changeVelocity(int16_t rightVelocity, int16_t leftVelocity)
{
   return {DRIVE_DIRECT_4, static_cast<uint8_t>(rightVelocity >> 8),
           static_cast<uint8_t>(rightVelocity & 0x00ff),
           static_cast<uint8_t>(leftVelocity >> 8),
           static_cast<uint8_t>(leftVelocity & 0x00ff)};
}

#endif
