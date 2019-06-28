#ifndef OPENINTERFACECONFIG_H
#define OPENINTERFACECONFIG_H

#include <cstdint>
#include <vector>

/**
 *  \class OIC defines all basic scripts for serial communication
 **/
class OIC
{
    public:
        OIC();
        ~OIC();
        
        const bool FOREVER;

        // Get started
        const uint8_t START;
        const uint8_t BAUD_1;

        // Mode commands
        const uint8_t MODE_SAFE;
        const uint8_t MODE_FULL;

        // Commands
        const uint8_t DRIVE_DIRECT_4;

        const std::vector<uint8_t> SENSORS;
        const std::vector<uint8_t> WALL_RIGHT;
        const std::vector<uint8_t> WALL_LEFT;
        const std::vector<uint8_t> WALL_ALL;

        const std::vector<uint8_t> startSafe();
        const std::vector<uint8_t> changeVelocity(int16_t rightVelocity, int16_t leftVelocity);
};
#endif
