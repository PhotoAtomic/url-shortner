import { ConfigurationService } from './configuration.service';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { firstValueFrom, lastValueFrom } from 'rxjs';



class Response{
  shortUrl:string|null=null;
}

@Injectable({
  providedIn: 'root'
})
export class ShortnerService {

  private options:any;

  constructor(private configuration:ConfigurationService, private http: HttpClient) {
    this.options = { headers: new HttpHeaders().set('Content-Type', 'application/json') };

  }

  async shorten(url: string) : Promise<string> {
    try{
      var response = await lastValueFrom(
        this.http.put<Response>(
          `${this.configuration.apiUrl}/Shorten`,
          JSON.stringify({url:url}),
          this.options
          ));
          console.log("[ShortnerService]","has responded",response);
      return "some x";
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
