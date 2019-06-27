INCLUDING = -Ilib/SerialLink
CFLAGS= -Wall -ggdb -Ilib/mqtt -Ilib/mqtt/cpp
LDFLAGS= -Llib/mqtt lib/mqtt/cpp/libmosquittopp.so.1 lib/mqtt/libmosquitto.so.1 -lboost_system -pthread

EXECUTABLE = controlRoomba
SOURCES = ${wildcard program/*.cpp} \
          ${wildcard lib/SerialLink/*.cpp}
HEADERS = ${wildcard program/*.h} \
          ${wildcard lib/SerialLink/*.h}
OBJECTS = ${SOURCES:.cpp=.o}

.PHONY: all
all: ${EXECUTABLE}

$(EXECUTABLE): $(OBJECTS) buildnumber.num
	$(CXX) $(CXXFLAGS) $(OBJECTS) $(LDFLAGS) -o $@
	@echo "-- Build: " $$(cat buildnumber.num)

# Create dependency file using the compiler option -MM
depend: $(SOURCES)
	$(CXX) $(CXXFLAGS) -MM  $^ > $@

-include depend

# Buildnumber in text file
buildnumber.num: $(OBJECTS)
	@if ! test -f buildnumber.num; then echo 0 > buildnumber.num; fi
	@echo $$(($$(cat buildnumber.num)+1)) > buildnumber.num

# Create a clean environment
.PHONY: clean
clean:
	$(RM) $(EXECUTABLE) $(OBJECTS)

# Clean up dependency file
.PHONY: clean-depend
clean-depend: clean
	$(RM) depend
