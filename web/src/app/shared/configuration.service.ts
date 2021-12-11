import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { IConfiguration } from './iConfiguration';


@Injectable({
  providedIn: 'root'
})
export class ConfigurationService implements IConfiguration{
  get apiUrl(){
    return environment.apiUrl;
  }
}
