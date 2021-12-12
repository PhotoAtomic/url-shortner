import { ShortnerService } from './../shared/shortner.service';
import { NgModule, Component, Injectable, Input } from '@angular/core';
import { HttpClientModule } from '@angular/common/http'

import { MatSliderModule } from '@angular/material/slider';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSpinner} from '@angular/material/progress-spinner'


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
  public executing:boolean = false;

  constructor(private shortner: ShortnerService) {}

  async short(urlToShort:string|null) {
    console.log("[SHORT]",urlToShort);
    if(this.executing) return;
    this.executing = true;
    if(urlToShort !==null ){
      if(urlToShort.trim()=="") {
        this.executing = false;
        return;
      }
      let shortenedUrl = await this.shortner.shorten(urlToShort);
      this.executions.push(new UrlPair(urlToShort,shortenedUrl))
      this.executing = false;
      return;
    }
    else{
      this.executing = false;
      return;
    }

  }
}


export class UrlPair{
  constructor(
    public longUrl:string="",
    public shortUrl:string="") {}
}
