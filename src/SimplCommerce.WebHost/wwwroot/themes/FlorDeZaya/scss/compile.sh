#!/bin/bash

#win version
#sass site.scss ../site.css --no-source-map --style=compressed

#ruby version
#sass site.scss ../site.css --sourcemap=none --style=compressed

#npm version
sass --style=compressed --no-source-map site.scss ../site.css