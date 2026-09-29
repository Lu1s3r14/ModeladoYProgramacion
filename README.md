#Chat con servidor

Para compilar el proyecto:
- Tener Meson instalado
- Tener .NET 8.0 SDK, para instalarlo: sudo dnf install dotnet-sdk-8.0

1- Hacer 'meson setup build'

2- compilar 'meson compile -C build

3- ejecutar el servidor: ./build/ServidorChat  <puerto>
4- ejecutar el cliente: ./build/ClienteChat <direccion ip> <puerto>
