import { Injectable } from '@angular/core';

@Injectable()
export class ConfigService {
    private authConfig = {
        tenant: 'clouddriversupervisor.onmicrosoft.com',
        clientId: 'b441228a-cb7f-44ab-9de6-f5a11c8f4f05',
        redirectUri: window.location.origin + '/login',
        resource: "https://graph.microsoft.com",
        endpoints: {
            "https://graph.microsoft.com": "https://graph.microsoft.com"
        }
    }  

    private fabricEndpoint = "https://cdsfabric.westeurope.cloudapp.azure.com";

    constructor() { }
 
    getFabricEndpoint(): string {
        return this.fabricEndpoint;
    }

    getAuthConfig(): any {
        return this.authConfig;
    }
}