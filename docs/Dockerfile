FROM jetbrains/writerside-builder:243.22562 AS build

ARG INSTANCE=Writerside/hopframe

RUN mkdir /opt/sources

WORKDIR /opt/sources

ADD Writerside ./Writerside

RUN export DISPLAY=:99 && Xvfb :99 & /opt/builder/bin/idea.sh helpbuilderinspect --source-dir /opt/sources --product $INSTANCE --runner other --output-dir /opt/wrs-output/

WORKDIR /opt/wrs-output

RUN unzip -O UTF-8 webHelpHOPFRAME2-all.zip -d /opt/wrs-output/unzipped-artifact

FROM httpd:2.4 AS http-server

COPY --from=build /opt/wrs-output/unzipped-artifact/ /usr/local/apache2/htdocs/