FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app

COPY ./publish/ .

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 COMPlus_JitTelemetry=0 COMPlus_legacyCorruptedStateExceptionsPolicy=1 COMPlus_legacyUnhandledExceptionPolicy=1

RUN adduser --system --home /app --shell /usr/sbin/nologin --no-create-home --disabled-password qexecution

CMD ["dotnet", "QExecute.ExecutionService.dll"]
