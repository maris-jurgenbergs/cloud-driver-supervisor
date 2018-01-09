export interface IAlertListResult {
    user: IUser;
    alerts: IAlert[];
}

export interface IAlertResult {
    user: IUser;
    alert: IAlert;
}

export interface IUser {
    azureId: string;
    name: string;
    surname: string;
    email: string;
    createdAt: number;
    roles: string[];
    phone: string;
}

export interface IAlert {
    alertId: string;
    status: AlertStatus;
    type: AlertType;
    severityLevel: SeverityLevel;
    createdAt: number;
    description: string;
}

export enum AlertStatus {
    Active = 0,
    Resolved = 1
}


export enum AlertType {
    Unknown = 0,
    RoadAccident = 1,
    HealthIssue = 2,
    Assault = 3,
    HeavyTraffic = 4,
    Other = 5
}


export enum SeverityLevel {
    Unevaluated = 0,
    Trivial = 1,
    Low = 2,
    Average = 3,
    High = 4,
    Critical = 5
}

export enum ViolationType {
    Week = 1,
    Day = 2,
    HalfDay = 3
}