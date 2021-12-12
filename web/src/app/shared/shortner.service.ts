import { ConfigurationService } from './configuration.service';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { firstValueFrom, lastValueFrom } from 'rxjs';
import { RequestDto } from '../dto/RequestDto';
import { ResponseDto } from '../dto/ResponseDto';


@Injectable({
  providedIn: 'root'
})
export class ShortnerService {

  private options:any;

  constructor(private configuration:ConfigurationService, private http: HttpClient) {}

  async shorten(url: string) : Promise<string> {
    try{
      let request = new RequestDto(url);

      let response = await lastValueFrom(
        this.http.put<ResponseDto>(
          `${this.configuration.apiUrl}/Shorten`,
          request
          ));
          console.log("[ShortnerService]","has responded",response);
      return response.shortUrl;
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
