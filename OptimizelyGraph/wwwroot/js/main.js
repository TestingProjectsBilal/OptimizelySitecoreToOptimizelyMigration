﻿import "bootstrap";
import "../scss/main.scss"
require("easy-autocomplete");
import "bootstrap-notify";
import feather from "feather-icons";
import "lazysizes";
import "lazysizes/plugins/bgset/ls.bgset";
import FoundationCms from "wwwroot/js/common/foundation.cms";
import FoundationCommerce from "wwwroot/js/common/foundation.commerce";
import FoundationPersonalization from "wwwroot/js/common/foundation.cms.personalization";

feather.replace();
window.feather = feather; 

let foundationCms = new FoundationCms();
foundationCms.init();

let foundationCommerce = new FoundationCommerce();
foundationCommerce.init();

let foundationPersonalization = new FoundationPersonalization();
foundationPersonalization.init();