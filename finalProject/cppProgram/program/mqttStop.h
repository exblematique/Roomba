#ifndef MQTTSTOP_H
#define MQTTSTOP_H

#include <mosquittopp.h>
#include <string>

/**
 *  \class mqttStop creates a connection with the broker to send a stop signal
 *  \note mqtt class uses the mosquittopp library
 **/
class mqttStop : public mosqpp::mosquittopp
{
	public:
        mqttStop() = delete;
        mqttStop(const char *id, const char *host, int port);
		~mqttStop();
        
        const void sendStop();
        const void sendWall();
        
    private:
        const std::string pubTopic;
        char buf[51];
        
        void on_connect(int rc);
};

#endif
