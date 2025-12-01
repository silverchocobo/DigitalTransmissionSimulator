# Simulador de Transmissão Digital

O presente projeto simula, através de um web app ASP.NET, a transmissão de um texto por um canal genérico, passando pelas principais etapas do enlace de uma comunicação digital: codificação, modulação de sinal, demodulação, decodificação e recepção da mensagem, além da adição de ruído gaussiano.

## Técnicas utilizadas

Optamos por usar apenas algumas das técnicas de codificação e modulação de sinal entre as diversas existentes. Em versões futuras do projeto, outras poderão ser implementadas.

As técnicas de codificação utilizadas foram:
- NRZ-Polar (Non-Return-to-Zero Polar):  bit 1 é mapeado para um nível de tensão positivo (+V) e o bit 0 para um nível de tensão negativo (-V). É eficiente em termos de largura de banda, mas pode ser prejudicado quando houver longas sequências de 0s ou 1s.
- AMI (Alternate Mark Inversion): Esquema ternário (+V, -V, 0V). Bit 0 é representado por 0V e o bit 1 alterna entre +V e -V. Sua principal vantagem é a ausência de corrente contínua (DC) e possui boa capacidade de correção de erros.

Para modulação do sinal, foram utilizadas as técnicas: 

- Modulação de Amplitude (para AMI): nível +1 transmite a portadora com amplitude positiva, -1 com amplitude negativa (fase invertida) e 0 transmite amplitude 0;
- BPSK (Binary Phase-Shift Keying): A fase da onda portadora é alterada de acordo com o bit de entrada. O bit 1 é transmitido com uma fase (0°) e o bit 0 é transmitido na fase oposta (180°).

## Execução do programa

### Web
A aplicação foi hospedada neste [website](https://digital-transmition-simulator-465687400990.southamerica-east1.run.app/).

### Local
1) Baixe e instale o runtime e o SDK da versão 8 do .NET através deste [link](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0).
2) Verifique a instalação digitando o comando `dotnet --version`. Você também pode listar os runtimes e SDK's instalados com `dotnet --list-sdks`
`dotnet --list-runtimes`
3) Vá para a pasta do projeto e digite `dotnet build`
4) Digite `dotnet run` e abra o link de localhost em seu navegador. Você está executando a aplicação localmente.
