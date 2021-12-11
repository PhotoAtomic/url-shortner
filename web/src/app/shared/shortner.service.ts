import { ConfigurationService } from './configuration.service';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { firstValueFrom, lastValueFrom } from 'rxjs';



@Injectable({
  providedIn: 'root'
})
export class ShortnerService {

  constructor(private configuration:ConfigurationService, private http: HttpClient) { }

  async shorten(url: string) : Promise<string> {
    try{
      var response = await lastValueFrom(
        this.http.put(
          `${this.configuration.apiUrl}/shorten`,
          JSON.stringify({url:url})));
          console.log("[ShortnerService]","has responded",response);
      return "shortned url";
    }
    catch(error){
      console.log("[ShortnerService]","has faulted");
      throw error;
    }
    finally{
      console.log("[ShortnerService]","has terminated");
    }
  }


}
