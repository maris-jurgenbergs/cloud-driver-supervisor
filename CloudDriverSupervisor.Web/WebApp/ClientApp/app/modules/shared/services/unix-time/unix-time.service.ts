import { Injectable } from '@angular/core';

@Injectable()
export class UnixTimeService {
    getUnixTime() : number{
        return new Date().getTime()/1000|0;
    }

    parseUnixTime(unixTime: number) : Date{
        return new Date(unixTime * 1000);
    }
}