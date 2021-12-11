import { ShortnerService } from './../shared/shortner.service';
import { NgModule, Component, Injectable } from '@angular/core';
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

  constructor(private shortner: ShortnerService) {}

  short() {
    this.shortner.shorten('http://localhost/BookLoan.Catalog.API/api/Book/List');

  }
}

