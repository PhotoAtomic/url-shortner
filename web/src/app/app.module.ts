
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { HttpClientModule } from '@angular/common/http'
import { ShortnerService } from './shared/shortner.service';
import { ShorterComponent } from './shorter/shorter.component';

import { MatSliderModule } from '@angular/material/slider';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { IConfiguration } from './shared/iConfiguration';
import { ConfigurationService } from './shared/configuration.service';

@NgModule({
  declarations: [
    AppComponent,
    ShorterComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,

    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSliderModule,

    RouterModule.forRoot([
      { path: '', component: ShorterComponent },
    ]),
    HttpClientModule,
  ],
  providers: [ConfigurationService, ShortnerService],
  bootstrap: [AppComponent]
})
export class AppModule { }
