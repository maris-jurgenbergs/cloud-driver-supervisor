export interface ITransportationResult {
    transportation: ITransportation;
    capturedLocations: ICapturedLocation[];
}

export interface ITransportation {
    transportationId: string;
}

export interface ICapturedLocation {
    altitude: number;
    longitude: number;
    capturedDateTimeUtc: Date;
}