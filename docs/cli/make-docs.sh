#!/bin/bash

rm -rf kic
rm -rf kicv
rm -rf flt
rm -rf flte

mkdir -p kic
mkdir -p kicv
mkdir -p flt
mkdir -p flte

kic --gen-docs
kicv --gen-docs
flt --gen-docs
flte --gen-docs

mv kic/kic.md kic/README.md
mv kicv/kic.md kicv/README.md
mv flt/flt.md flt/README.md
mv flte/flt.md flte/README.md
