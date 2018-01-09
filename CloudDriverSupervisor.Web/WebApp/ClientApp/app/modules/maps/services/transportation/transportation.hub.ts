import { Injectable } from "@angular/core";
import { HubConnection, LogLevel } from '@aspnet/signalr-client';

@Injectable()
export class TransportationHub {
    getHubConnection() {
        let homepage = "https://cdsfabric.westeurope.cloudapp.azure.com";
        //let homepage = "http://localhost";
        return new HubConnection(homepage + ":8419/transportation", { logging: LogLevel.Trace });
    }
}