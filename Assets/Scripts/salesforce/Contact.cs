using System;
using System.Collections.Generic;
using Boomlagoon.JSON;
using Salesforce;

public class Contact : SalesforceRecord {

    public const string BASE_QUERY = "SELECT Id, Name, public_avatar_link__c FROM Contact";

    public string Name { get; set; }
    public string public_avatar_link__c { get; set; }

    public Contact() {}

    public Contact(string id, string Name, string public_avatar_link__c) : base(id) {
        this.Name = Name;
        this.public_avatar_link__c = public_avatar_link__c;
    }

    public override string getSObjectName() {
        return "Contact";
    }

    public override JSONObject toJson() {
        JSONObject record = base.toJson();
        record.Add("Name", Name);
        record.Add("public_avatar_link__c", public_avatar_link__c);
        return record;
    }

    public override void parseFromJson(JSONObject jsonObject) {
        base.parseFromJson(jsonObject);
        Name = jsonObject.GetString("Name");
        public_avatar_link__c = jsonObject.GetString("public_avatar_link__c");
    }
}