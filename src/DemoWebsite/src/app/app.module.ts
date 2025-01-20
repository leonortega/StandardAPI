import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

@NgModule({
  declarations: [AppComponent], // only AppComponent
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule // sets up the lazy-loaded routes
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
