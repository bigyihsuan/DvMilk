#/bin/bash
cp "./bin/Release/DvMilk.dll" "./info.json" "./DvMilk/"

version=$(grep 'Version:*' "DvMilk/info.json" | cut -d':' -f2 | python3 build.py)

zip -r ../DvMilk_$version.zip DvMilk