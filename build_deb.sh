#!/bin/bash

set -e

APPNAME="vpnClientApp"
VERSION="1.0.0"
OUTDIR="${APPNAME}_${VERSION}"
PUBLISH_DIR="publish"
ICON_SRC="desktop_icon.png"
ICON_DST="usr/share/icons/hicolor/256x256/apps/${APPNAME}.png"
DESKTOP_FILE="usr/share/applications/${APPNAME}.desktop"
CONTROL_FILE="DEBIAN/control"

# 1. публікація .NET self-contained
dotnet publish -c Release -r linux-x64 --self-contained true -o $PUBLISH_DIR

# 2. структура .deb
rm -rf $OUTDIR
mkdir -p $OUTDIR/DEBIAN
mkdir -p $OUTDIR/usr/local/bin
mkdir -p $OUTDIR/usr/share/applications
mkdir -p $OUTDIR/usr/share/icons/hicolor/256x256/apps

# 3. копіювання
cp -r $PUBLISH_DIR/* $OUTDIR/usr/local/bin/
chmod +x $OUTDIR/usr/local/bin/$APPNAME

# 4. control
cat > $OUTDIR/$CONTROL_FILE <<EOF
Package: $APPNAME
Version: $VERSION
Section: net
Priority: optional
Architecture: amd64
Maintainer: Твоє Ім'я <email@example.com>
Description: ZoogVPN offers freedom, privacy, and security online. ZoogVPN is a top-rated commercial VPN with a free VPN plan and premium VPN services. Get a leading VPN with blazing-fast speed for unlimited browsing while staying safe and private online anywhere in the world.
EOF

# 5. desktop-файл
cat > $OUTDIR/$DESKTOP_FILE <<EOF
[Desktop Entry]
Name=Zoog VPN: Secure VPN & Proxy
Exec=/usr/local/bin/$APPNAME
Icon=$APPNAME
Type=Application
Categories=Network;Utility;
EOF

# 6. іконка
cp $ICON_SRC $OUTDIR/$ICON_DST

# 7. збірка
dpkg-deb --build $OUTDIR

echo "Готово! Файл $OUTDIR.deb створено."
echo "Встановити: sudo dpkg -i $OUTDIR.deb"
