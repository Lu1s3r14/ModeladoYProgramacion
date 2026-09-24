project('Proyecto1', version: '1.0')
dotnet = find_program('dotnet', required: true)
custom_target('compilar_servidor',
  build_by_default: true,
  input: 'src/servidor/Servidor.csproj',
  output: 'ServidorChat',
  command: [
    dotnet, 'publish', '@INPUT@',
    '-c', 'Release',
    '-r', 'linux-x64',
    '-o', '@OUTDIR@'
  ]
  )

custom_target('compilar_cliente',
  build_by_default: true,
  input: 'src/cliente/Cliente.csproj',
  output: 'ClienteChat',
  command: [
  dotnet, 'publish', '@INPUT@',
  '-c', 'Release',
  '-r', 'linux-x64',
  '-o', '@OUTDIR@'
  ]
  ) 