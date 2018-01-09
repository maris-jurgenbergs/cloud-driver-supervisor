import { Map, tileLayer, latLng } from 'leaflet';
import { Component } from '@angular/core';

import { MapService } from '../services/maps.service';

@Component({
    templateUrl: './maps.page.html',
    styleUrls: ['./maps.page.css']
})
export class MapsComponent {
    options = {
        layers: [
            tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 18, attribution: '...' })
        ],
        zoom: 12,
        center: latLng(56.933737, 24.145889)
    };

    constructor(private mapService: MapService) {}
    
    onMapReady(map: Map) {        
        this.mapService.initialize(map);
    }
}
