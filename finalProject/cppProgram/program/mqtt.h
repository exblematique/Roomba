#ifndef MQTT_H
#define MQTT_H

#include <mosquittopp.h>
#include "command.h"

class mqtt : public mosqpp::mosquittopp
{
	public:
		mqtt(const char *id, const char *host, int port);
		~mqtt();

		void on_connect(int rc);
		void on_message(const struct mosquitto_message *message);
		void on_subscribe(int mid, int qos_count, const int *granted_qos);
        
    private:
        command cd;
        const std::string rootTopic;
        const std::string pubTopic;
};

#endif
