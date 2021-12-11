import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';



@Injectable({
  providedIn: 'root'
})
export class ShortnerService {

  constructor(private http: HttpClient) { }

  shorten(url: string) {

    window.alert('The link will be shortened');
  }


}
