#ifndef MQTT_H
#define MQTT_H

#include <mosquittopp.h>

#include "command.h"

/**
 *  \class mqtt creates a connection with the broker and determine next action according to incoming message
 *  \note mqtt class uses the mosquittopp library
 **/
class mqtt : public mosqpp::mosquittopp
{
	public:
        mqtt() = delete;
		mqtt(const char *id, const char *host, int port);
		~mqtt();
        
        const void sendStop();
        
    private:
        command cd;
        const std::string rootTopic;
        const std::string pubTopic;
        
        void on_connect(int rc);
		void on_message(const struct mosquitto_message *message);
		void on_subscribe(int mid, int qos_count, const int *granted_qos);
};


#endif
