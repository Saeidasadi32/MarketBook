# DOC-0008 — MarketPrice Domain Model

MarketPrice is the persisted official daily price record for one Listing and one TradingDate.

Invariants:
- unique `(ListingId, TradingDate)`
- all prices non-negative
- `LowPrice <= HighPrice`
- `LowerLimit <= UpperLimit`
- Open/High/Low/Last/Close must remain inside the permitted daily limits
- Listing must exist and be active at creation

The first slice persists OHLC, last/close, previous close, reference price, and lower/upper price limits. Volume, trade statistics, order book and tick snapshots remain Phase 4 work.
