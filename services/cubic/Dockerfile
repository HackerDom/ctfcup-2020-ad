FROM python:3.8

RUN apt update && \
    apt install -y socat libgmp3-dev

RUN useradd -s /bin/false -m cubic

WORKDIR /home/cubic/

ADD requirements.txt ./

RUN pip install -r requirements.txt

COPY service /home/cubic

RUN chmod 555 ./main.py

ENTRYPOINT ["socat", "TCP-LISTEN:31337,reuseaddr,fork", "EXEC:'timeout 300 ./main.py',su=cubic"]