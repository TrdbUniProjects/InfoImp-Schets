all: schets.tar.gz

CS_SOURCE := $(shell find -type f -name \*.cs)

schets.tar.gz: ${CS_SOURCE}
	rm -rf obj bin
	tar -czf schets.tar.gz *

.PHONY: clean
clean:
	rm -rf obj bin schets.tar.gz
