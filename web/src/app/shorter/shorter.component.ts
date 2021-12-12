import { ShortnerService } from './../shared/shortner.service';
import { NgModule, Component, Injectable, Input } from '@angular/core';
import { HttpClientModule } from '@angular/common/http'

import { MatSliderModule } from '@angular/material/slider';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';


import { HttpClient, HttpHeaders } from '@angular/common/http'


@Component({
  selector: 'shorter',
  templateUrl: './shorter.component.html',
  styleUrls: ['./shorter.component.css'],
})

@Injectable({
  providedIn: 'root'
})
export class ShorterComponent {

  public executions:UrlPair[]=[];

  constructor(private shortner: ShortnerService) {}

  async short(urlToShort:string|null) {
    console.log("[SHORT]",urlToShort);
    if(urlToShort !==null){
      let shortenedUrl = await this.shortner.shorten(urlToShort);
      this.executions.push(new UrlPair(urlToShort,shortenedUrl))
      return;
    }
    else{
      return;
    }

  }
}


export class UrlPair{
  constructor(
    public longUrl:string="",
    public shortUrl:string="") {}
}
