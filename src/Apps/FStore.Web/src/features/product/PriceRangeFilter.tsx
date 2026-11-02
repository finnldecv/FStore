import { useState, useEffect } from "react";
import Slider from "rc-slider";
import 'rc-slider/assets/index.css';
import { Form } from "react-bootstrap";
import { useDebounce } from "use-debounce";

interface PriceRangeFilterProps {
  min: number;
  max: number;
  onChange: (min: number, max: number) => void;
}

export const PriceRangeFilter = ({ min, max, onChange }: PriceRangeFilterProps) => {
  const [range, setRange] = useState<[number, number]>([min, max]);
  const [debouncedRange] = useDebounce(range, 500);

  useEffect(() => {
    if (min !== range[0] || max !== range[1]) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setRange([min, max]);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [min, max])

  useEffect(() => {
    onChange(debouncedRange[0], debouncedRange[1]);
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [debouncedRange]);

  return (
    <div className="mb-4 p-3 bg-light rounded" style={{ maxWidth: '400px' }}>
      <Form.Label className="d-flex justify-content-between">
        <span>Price</span>
        <span className="fw-bold text-primary">
          {range[0].toLocaleString('en-US')} $ - {range[1].toLocaleString('en-US')} $
        </span>
      </Form.Label>
      <Slider
        range
        min={0}
        max={2500}
        step={50}
        value={range}
        onChange={(value) => setRange(value as [number, number])}
        styles={{
          track: { backgroundColor: '#0d6efd' },
          handle: { backgroundColor: '#0d6efd', borderColor: '#0d6efd' }
        }}

      />
    </div>
  );
}