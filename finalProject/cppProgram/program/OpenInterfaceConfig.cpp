#include <cstdint>
#include <vector>

#include "OpenInterfaceConfig.h"


/**
 *  \brief OIC::OIC creates variables
 **/
OIC::OIC()
    : FOREVER{true}
    , START{128}
    , BAUD_1{129}
    , MODE_SAFE{131}
    , MODE_FULL{132}
    , DRIVE_DIRECT_4{145}
    , SENSORS{142, 7}
    , WALL_RIGHT{19, 1}
    , WALL_LEFT{19, 2}
    , WALL_ALL{19, 3}
{
}

/**
 *  \brief OIC::OIC~ delete the current object
 *  \note This function is not used
 **/
OIC::~OIC()
{
}
/**
 *  \brief startSafe initializes the roomba
 *  \return std::vector<uint8_t> which contains all numbers to send to Roomba
 **/
const std::vector<uint8_t> OIC::startSafe()
{
   return {START, MODE_SAFE};
}

/**
 *  \brief changeVelocity return serial syntax to define the new velocity 
 *  \param rightVelocity is the speed of right wheel with a signed number
 *  \param leftVelocity is the speed of left wheel with a signed number
 *  \return std::vector<uint8_t> which contains all numbers to send to Roomba
 **/
const std::vector<uint8_t> OIC::changeVelocity(int16_t rightVelocity, int16_t leftVelocity)
{
   return {DRIVE_DIRECT_4, static_cast<uint8_t>(rightVelocity >> 8),
           static_cast<uint8_t>(rightVelocity & 0x00ff),
           static_cast<uint8_t>(leftVelocity >> 8),
           static_cast<uint8_t>(leftVelocity & 0x00ff)};
}
