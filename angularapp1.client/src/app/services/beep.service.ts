import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class BeepService {
  private ctx?: AudioContext;

  private get audio(): AudioContext {
    if (!this.ctx) this.ctx = new (window.AudioContext || (window as any).webkitAudioContext)();
    return this.ctx!;
  }

  /** Pitido corto (freq=1000Hz, 300ms) */
  beep(frequency = 1000, durationMs = 300, volume = 0.2) {
    const ctx = this.audio;
    const osc = ctx.createOscillator();
    const gain = ctx.createGain();
    osc.type = 'sine';
    osc.frequency.value = frequency;
    gain.gain.value = volume;

    osc.connect(gain).connect(ctx.destination);
    osc.start();

    // envelope simple para evitar clics
    const now = ctx.currentTime;
    gain.gain.setValueAtTime(0, now);
    gain.gain.linearRampToValueAtTime(volume, now + 0.01);
    gain.gain.linearRampToValueAtTime(0, now + durationMs / 1000);

    osc.stop(now + durationMs / 1000 + 0.01);
  }
  beepSequence(times: number, frequency = 700, durationMs = 250, gapMs = 300, volume = 0.25) {
    for (let i = 0; i < times; i++) {
      setTimeout(() => this.beep(frequency, durationMs, volume), i * (durationMs + gapMs));
    }
  }
}
