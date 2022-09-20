//
// Created by DasDarki on 20.09.2022.
//

#ifndef NANOSSHARP_RUNTIME_ICVALUES_H
#define NANOSSHARP_RUNTIME_ICVALUES_H

#include <vector>

enum ICValueType : uint8_t {
    Null,
    Boolean,
    Integer,
    Double,
    String,
    Pointer,
    Array, // The array can contain any IC value type
};

/**
 * An IC value or interchangable value is a value that can be passed to the managed side without corrupting
 * the memory. This is done by just passing the pointer to the value and the type of the value and keep the
 * management of the value on the unmanaged side.
 */
class ICValue {
protected:
    ICValueType m_Type;

public:
    ICValue(ICValueType type) : m_Type(type) {}
    virtual ~ICValue() = default;

    ICValueType GetType() const { return m_Type; }
};

class ICNullValue : public ICValue {
public:
    ICNullValue() : ICValue(ICValueType::Null) {}
};

class ICBooleanValue : public ICValue {
public:
    ICBooleanValue(bool value) : ICValue(ICValueType::Boolean), Value(value) {}

    bool Value;
};

class ICIntegerValue : public ICValue {
public:
    ICIntegerValue(long long value) : ICValue(ICValueType::Integer), Value(value) {}

    long long Value;
};

class ICDoubleValue : public ICValue {
public:
    ICDoubleValue(double value) : ICValue(ICValueType::Double), Value(value) {}

    double Value;
};

class ICStringValue : public ICValue {
public:
    ICStringValue(const std::string &value) : ICValue(ICValueType::String), Value(value) {}

    const std::string &Value;
};

class ICPointerValue : public ICValue {
public:
    ICPointerValue(void *value) : ICValue(ICValueType::Pointer), Value(value) {}

    void *Value;
};

class ICArrayValue : public ICValue {
public:
    ICArrayValue(std::vector<ICValue *> &value) : ICValue(ICValueType::Array), Value(value) {}

    std::vector<ICValue *> &Value;
};

#endif //NANOSSHARP_RUNTIME_ICVALUES_H
