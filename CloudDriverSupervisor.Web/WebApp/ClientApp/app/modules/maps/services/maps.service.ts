import 'leaflet-polylinedecorator';
import 'leaflet-routing-machine';
import * as Line from '../dist/line';

import { Injectable } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { Map } from 'leaflet';
import { MatDialog } from '@angular/material';

import { TransportationHub } from './transportation/transportation.hub';
import { TransportationService } from './transportation/transportation.service';
import { MapTransportationDialogComponent } from '../components/map-transportation-dialog/map-transportation-dialog.component';

declare var L: any;

@Injectable()
export class MapService{
    static dialog: any;
    private hubConnection: HubConnection;
    private localMap: Map;
    private transportationControls: { [id: string]: any; };
    private lorryIconControls: { [id: string]: any };

    constructor(private transportationService: TransportationService, private transportationHub: TransportationHub, private dialog: MatDialog) {
        MapService.dialog = dialog;
    }

    initialize(map: Map) {
        this.localMap = map;
        this.transportationControls = {};
        this.lorryIconControls = {};
        this.transportationService.getSasUri().subscribe(data => {
            this.handleNewTransportations(data.payloadSasUri);
        });
    }
    private startHubConnection() {
        this.hubConnection = this.transportationHub.getHubConnection();
        this.hubConnection.on("updateTransportationList", (data: string) => {
            var result = JSON.parse(data);
            for (let i = 0; i < result.length; i++) {
                this.handleNewTransportations(result[i]);
            }
        });
        this.hubConnection.start()
            .then(() => {
                console.log('Hub connection started');
            })
            .catch(err => {
                console.log('Error while establishing connection');
            });
    }

    private handleNewTransportations(sasUri: string) {
        this.transportationService.getTransportations(sasUri).subscribe(data => {
            console.log(data);
            let waypoints: {}[] = [];
            for (let i = 0; i < data.length; i++) {
                let tempCoordinates: any[] = [];
                data[i].capturedLocations.sort(this.compareCapturedLocations);
                for (let b = 0; b < data[i].capturedLocations.length && b < 50; b++) {
                    let lattLng = L.latLng(
                        data[i].capturedLocations[b].altitude,
                        data[i].capturedLocations[b].longitude);
                    tempCoordinates.push(lattLng);
                }
                waypoints.push({
                    transportationId: data[i].transportation.transportationId,
                    coordinates: tempCoordinates
                });
            }
            this.addTransportationsToMap(this.localMap, waypoints);
        });
    }

    private compareCapturedLocations(a: any, b: any) {
        const coordA = a.capturedDateTimeUtc;
        const coordB = b.capturedDateTimeUtc;

        let comparison = 0;
        if (coordA > coordB) {
            comparison = 1;
        } else if (coordA < coordB) {
            comparison = -1;
        }
        return comparison;
    }

    private addTransportationsToMap(map: Map, waypoints: any) {
        for (let i = 0; i < waypoints.length; i++) {
            let control = this.transportationControls[waypoints[i].transportationId];
            if (control) {
                let coodinateCount = control.getWaypoints().length;
                for (let x = 0; x < waypoints[i].coordinates.length; x++) {
                    control.spliceWaypoints(coodinateCount, 0, waypoints[i].coordinates[x]);
                    coodinateCount++;
                }
            } else {
                let inlineLorryIconControls = this.lorryIconControls;
                control = L.Routing.control({
                    fitSelectedRoutes: false,
                        waypoints: waypoints[i].coordinates,
                        show: false,
                        draggableWaypoints: false,
                        addWaypoints: false,
                        lineOptions: {
                            styles: [{ color: this.getRandomColor(), opacity: 1 }]
                        },
                        createMarker: function () { return null; },
                        routeLine(route: any, options: any) {
                            let lorryIconControl = inlineLorryIconControls[waypoints[i].transportationId];
                            if (lorryIconControl) {
                                map.removeLayer(lorryIconControl);
                            } 
                                lorryIconControl = L.polylineDecorator(route.coordinates,
                                    {
                                        patterns: [
                                            {
                                                offset: '100%',
                                                //repeat: '1px',
                                                symbol: L.Symbol.marker({
                                                    rotate: true,
                                                    markerOptions: {
                                                        icon: L.icon({
                                                            iconUrl: 'lorry.png',
                                                            iconAnchor: [8, 20.5]
                                                        }),
                                                        clickable: true
                                                    }
                                                })
                                            }
                                        ]
                                    });
                                    
                                    var coordinates = route.coordinates[route.coordinates.length - 1];
                                    var myIcon = L.icon({
                                        iconUrl: 'lorry-hover-effect.png',
                                         iconSize: [40, 40],
                                    });
                                    let hoverMarkerOptions = {
                                        icon: myIcon,
                                        zIndexOffset: 3000
                                    };
                                    var lorryMarker = L.marker(coordinates, hoverMarkerOptions);
                                    
                                    //  lorryMarker.addTo(map).on('click', onLorryClick);
                                    lorryMarker.addTo(map).on('click', function(event: any) { 
                                        onLorryClick(event.target.transportationId);
                                    });
                                    lorryMarker.transportationId = waypoints[i].transportationId;
                                inlineLorryIconControls[waypoints[i].transportationId] = lorryIconControl;
                                lorryIconControl.addTo(map);

                            return new Line(route, options);
                        },
                    });
                    this.transportationControls[waypoints[i].transportationId] = control;
                    control.addTo(map);
                }
        }
    }
    
    static openTransportationDetails(transportationId: any) {
        let dialogRef = MapService.dialog.open(MapTransportationDialogComponent, {
            width: '500px',
            data: {
                transportationId
            },
          });
    }

    private getRandomColor() {
        var letters = '0123456789ABCDEF'.split('');
        var color = '#';
        for (var i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    }
}

function onLorryClick(transportationId: any) {
    MapService.openTransportationDetails(transportationId);
}