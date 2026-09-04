project('Prueba', version: '1.0')
dotnet = find_program('dotnet', required: true)
custom_target('compilar_dotnet_aot',
  build_by_default: true,
  input: 'hola.csproj',
  output: 'ProyectoAOT',
  command: [
    dotnet, 'publish', '@INPUT@',
    '-c', 'Release',
    '-r', 'linux-x64',
    '-o', '@OUTDIR@'
  ]
  )